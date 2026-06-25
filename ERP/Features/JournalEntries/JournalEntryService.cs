using Erp.Module.GL.Data;
using Erp.Module.GL.Entities;
using ERP.Features.JournalEntries.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erp.Shared.Interfaces;
using Erp.Shared.Enums;

namespace ERP.Features.JournalEntries
{
    public class JournalEntryService : IJournalEntryService
    {
        private readonly GlDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public JournalEntryService(GlDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<JournalEntryDto>> GetAllAsync()
        {
            var entries = await _context.JournalEntries
                .Include(j => j.Lines)
                .ThenInclude(l => l.GlAccount)
                .OrderByDescending(j => j.Date)
                .ToListAsync();

            return entries.Select(MapToDto).ToList();
        }

        public async Task<JournalEntryDto?> GetByIdAsync(Guid id)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.Lines)
                .ThenInclude(l => l.GlAccount)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) return null;

            return MapToDto(entry);
        }

        public async Task<JournalEntryDto> CreateAsync(CreateJournalEntryDto dto)
        {
            var tenantId = _currentUserService.TenantId;
            if (tenantId == null) throw new UnauthorizedAccessException("Tenant ID is required.");

            // 1. Validation: Debits must equal Credits
            decimal totalDebit = dto.Lines.Sum(l => l.Debit);
            decimal totalCredit = dto.Lines.Sum(l => l.Credit);

            if (totalDebit != totalCredit)
            {
                throw new InvalidOperationException($"Journal Entry is unbalanced. Total Debit: {totalDebit}, Total Credit: {totalCredit}");
            }

            if (totalDebit == 0)
            {
                throw new InvalidOperationException("Journal Entry must have a non-zero value.");
            }

            // 2. Validation: Accounts exist and allow manual entry
            var accountIds = dto.Lines.Select(l => l.GlAccountId).Distinct().ToList();
            var accounts = await _context.GlAccounts
                .Where(a => accountIds.Contains(a.Id) && a.IsActive)
                .ToDictionaryAsync(a => a.Id);

            foreach (var lineDto in dto.Lines)
            {
                if (!accounts.TryGetValue(lineDto.GlAccountId, out var account))
                {
                    throw new InvalidOperationException($"Account with ID {lineDto.GlAccountId} does not exist or is inactive.");
                }

                if (account.PostingType != GlPostingType.Posting)
                {
                    throw new InvalidOperationException($"Account {account.AccountNumber} is a Header account and cannot be posted to.");
                }

                // If the user is manually creating this via the API, the account must allow manual entries.
                // Note: In the future, system-generated journals (e.g. from Sales module) can bypass this.
                if (!account.AllowManualEntry)
                {
                    throw new InvalidOperationException($"Account {account.AccountNumber} does not allow manual journal entries.");
                }

                if (lineDto.Debit < 0 || lineDto.Credit < 0)
                {
                    throw new InvalidOperationException("Debit and Credit amounts cannot be negative.");
                }
                
                if (lineDto.Debit > 0 && lineDto.Credit > 0)
                {
                    throw new InvalidOperationException($"Line for account {account.AccountNumber} cannot have both a Debit and a Credit.");
                }
            }

            // Create Entity
            var journalEntry = new JournalEntry
            {
                TenantId = tenantId.Value,
                Date = dto.Date,
                Reference = dto.Reference,
                Description = dto.Description,
                IsPosted = dto.PostImmediately,
                CreatedAt = DateTime.UtcNow
            };

            foreach (var lineDto in dto.Lines)
            {
                journalEntry.Lines.Add(new JournalEntryLine
                {
                    TenantId = tenantId.Value,
                    GlAccountId = lineDto.GlAccountId,
                    Debit = lineDto.Debit,
                    Credit = lineDto.Credit,
                    TaxCodeId = lineDto.TaxCodeId,
                    DimensionId = lineDto.DimensionId,
                    CreatedAt = DateTime.UtcNow
                });
            }

            _context.JournalEntries.Add(journalEntry);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(journalEntry.Id) ?? throw new Exception("Failed to retrieve created journal entry.");
        }

        public async Task<bool> PostAsync(Guid id)
        {
            var entry = await _context.JournalEntries.FindAsync(id);
            if (entry == null) return false;

            if (entry.IsPosted)
                throw new InvalidOperationException("Journal Entry is already posted.");

            entry.IsPosted = true;
            entry.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        private JournalEntryDto MapToDto(JournalEntry entry)
        {
            return new JournalEntryDto
            {
                Id = entry.Id,
                Date = entry.Date,
                Reference = entry.Reference,
                Description = entry.Description,
                IsPosted = entry.IsPosted,
                Lines = entry.Lines.Select(l => new JournalEntryLineDto
                {
                    Id = l.Id,
                    GlAccountId = l.GlAccountId,
                    GlAccountNumber = l.GlAccount?.AccountNumber ?? "",
                    GlAccountName = l.GlAccount?.AccountName ?? "",
                    Debit = l.Debit,
                    Credit = l.Credit,
                    TaxCodeId = l.TaxCodeId,
                    DimensionId = l.DimensionId
                }).ToList()
            };
        }
    }
}
