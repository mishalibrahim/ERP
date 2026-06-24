using Erp.Module.GL.Data;
using Erp.Module.GL.Entities;
using ERP.Features.GlAccounts.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Erp.Shared.Interfaces;
using Erp.Shared.Enums;

namespace ERP.Features.GlAccounts
{
    public class GlAccountService : IGlAccountService
    {
        private readonly GlDbContext _context;
        private readonly ICurrentUserService _currentUserService;

        public GlAccountService(GlDbContext context, ICurrentUserService currentUserService)
        {
            _context = context;
            _currentUserService = currentUserService;
        }

        public async Task<List<GlAccountDto>> GetAllAsync()
        {
            return await _context.GlAccounts
                .Where(a => a.IsActive)
                .Select(a => new GlAccountDto
                {
                    Id = a.Id,
                    AccountNumber = a.AccountNumber,
                    AccountName = a.AccountName,
                    AccountType = a.AccountType,
                    AccountCategory = a.AccountCategory,
                    PostingType = a.PostingType,
                    AllowManualEntry = a.AllowManualEntry,
                    MandatoryDimensions = a.MandatoryDimensions,
                    ParentAccountId = a.ParentAccountId,
                    DefaultTaxGroupId = a.DefaultTaxGroupId,
                    IsActive = a.IsActive
                })
                .OrderBy(a => a.AccountNumber)
                .ToListAsync();
        }

        public async Task<List<GlAccountTreeNodeDto>> GetTreeAsync()
        {
            var allAccounts = await _context.GlAccounts
                .Where(a => a.IsActive)
                .OrderBy(a => a.AccountNumber)
                .ToListAsync();

            var dtos = allAccounts.Select(a => new GlAccountTreeNodeDto
            {
                Id = a.Id,
                AccountNumber = a.AccountNumber,
                AccountName = a.AccountName,
                AccountType = a.AccountType,
                AccountCategory = a.AccountCategory,
                PostingType = a.PostingType,
                AllowManualEntry = a.AllowManualEntry,
                MandatoryDimensions = a.MandatoryDimensions,
                ParentAccountId = a.ParentAccountId,
                DefaultTaxGroupId = a.DefaultTaxGroupId,
                IsActive = a.IsActive
            }).ToList();

            var lookup = dtos.ToDictionary(a => a.Id);
            var rootNodes = new List<GlAccountTreeNodeDto>();

            foreach (var dto in dtos)
            {
                if (dto.ParentAccountId.HasValue && lookup.TryGetValue(dto.ParentAccountId.Value, out var parent))
                {
                    parent.Children.Add(dto);
                }
                else
                {
                    rootNodes.Add(dto);
                }
            }

            return rootNodes;
        }

        public async Task<string> GetNextAccountNumberAsync(GlAccountCategory category, Guid? parentId)
        {
            string prefix;
            if (parentId.HasValue)
            {
                var parent = await _context.GlAccounts.FindAsync(parentId.Value);
                if (parent != null)
                {
                    prefix = parent.AccountNumber.TrimEnd('0');
                }
                else
                {
                    prefix = GetCategoryStartDigit(category).ToString();
                }
            }
            else
            {
                prefix = GetCategoryStartDigit(category).ToString();
            }

            // Find max account number starting with this prefix
            var maxAccount = await _context.GlAccounts
                .Where(a => a.IsActive && a.AccountNumber.StartsWith(prefix))
                .Select(a => a.AccountNumber)
                .OrderByDescending(a => a)
                .FirstOrDefaultAsync();

            if (maxAccount == null)
            {
                // If parent is 110000, prefix is 11, we return 111000
                return prefix.PadRight(6, '0').Substring(0, prefix.Length) + "1".PadRight(6 - prefix.Length, '0');
            }

            if (int.TryParse(maxAccount, out int maxNum))
            {
                // Simple increment by 1000 or logic based on position. Let's just do +1000 for standard spacing.
                return (maxNum + 1000).ToString().PadLeft(6, '0');
            }

            return prefix.PadRight(6, '0');
        }

        private char GetCategoryStartDigit(GlAccountCategory category) => category switch
        {
            GlAccountCategory.Asset => '1',
            GlAccountCategory.Liability => '2',
            GlAccountCategory.Equity => '3',
            GlAccountCategory.Income => '4',
            GlAccountCategory.Expense => '5',
            _ => throw new InvalidOperationException("Invalid category.")
        };

        private void ValidateAccountNumber(string accountNumber, GlAccountCategory category)
        {
            if (string.IsNullOrWhiteSpace(accountNumber) || accountNumber.Length != 6 || !accountNumber.All(char.IsDigit))
                throw new InvalidOperationException("Account number must be exactly 6 digits.");

            char expectedStartDigit = GetCategoryStartDigit(category);

            if (accountNumber[0] != expectedStartDigit)
                throw new InvalidOperationException($"Account number for category {category} must start with {expectedStartDigit}.");
        }

        private async Task ValidateHierarchyAsync(Guid? parentId, GlAccountCategory childCategory, string childAccountNumber)
        {
            if (!parentId.HasValue) return;

            var parent = await _context.GlAccounts.FindAsync(parentId.Value);
            if (parent == null || !parent.IsActive)
                throw new InvalidOperationException("Parent account not found or is inactive.");

            if (parent.AccountCategory != childCategory)
                throw new InvalidOperationException("Child account must have the same category as the parent account.");
        }

        public async Task<GlAccountDto?> GetByIdAsync(Guid id)
        {
            var account = await _context.GlAccounts
                .Where(a => a.Id == id && a.IsActive)
                .FirstOrDefaultAsync();

            if (account == null) return null;

            return new GlAccountDto
            {
                Id = account.Id,
                AccountNumber = account.AccountNumber,
                AccountName = account.AccountName,
                AccountType = account.AccountType,
                AccountCategory = account.AccountCategory,
                PostingType = account.PostingType,
                AllowManualEntry = account.AllowManualEntry,
                MandatoryDimensions = account.MandatoryDimensions,
                ParentAccountId = account.ParentAccountId,
                DefaultTaxGroupId = account.DefaultTaxGroupId,
                IsActive = account.IsActive
            };
        }

        public async Task<GlAccountDto> CreateAsync(CreateGlAccountDto dto)
        {
            var tenantId = _currentUserService.TenantId;
            if (tenantId == null) throw new UnauthorizedAccessException("Tenant ID is required.");

            ValidateAccountNumber(dto.AccountNumber, dto.AccountCategory);
            await ValidateHierarchyAsync(dto.ParentAccountId, dto.AccountCategory, dto.AccountNumber);

            var exists = await _context.GlAccounts
                .AnyAsync(a => a.AccountNumber == dto.AccountNumber);
            if (exists)
                throw new InvalidOperationException($"Account number {dto.AccountNumber} already exists.");

            var account = new GlAccount
            {
                TenantId = tenantId.Value,
                AccountNumber = dto.AccountNumber,
                AccountName = dto.AccountName,
                AccountType = dto.AccountType,
                AccountCategory = dto.AccountCategory,
                PostingType = dto.PostingType,
                AllowManualEntry = dto.AllowManualEntry,
                MandatoryDimensions = dto.MandatoryDimensions,
                ParentAccountId = dto.ParentAccountId,
                DefaultTaxGroupId = dto.DefaultTaxGroupId,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.GlAccounts.Add(account);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(account.Id) ?? throw new Exception("Failed to retrieve created account.");
        }

        public async Task<GlAccountDto?> UpdateAsync(Guid id, UpdateGlAccountDto dto)
        {
            var account = await _context.GlAccounts
                .Where(a => a.Id == id && a.IsActive)
                .FirstOrDefaultAsync();

            if (account == null) return null;

            await ValidateHierarchyAsync(dto.ParentAccountId, dto.AccountCategory, account.AccountNumber);

            account.AccountName = dto.AccountName;
            account.AccountType = dto.AccountType;
            account.AccountCategory = dto.AccountCategory;
            account.PostingType = dto.PostingType;
            account.AllowManualEntry = dto.AllowManualEntry;
            account.MandatoryDimensions = dto.MandatoryDimensions;
            account.ParentAccountId = dto.ParentAccountId;
            account.DefaultTaxGroupId = dto.DefaultTaxGroupId;
            account.IsActive = dto.IsActive;
            account.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            return await GetByIdAsync(account.Id);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var account = await _context.GlAccounts
                .Include(a => a.SubAccounts)
                .Where(a => a.Id == id && a.IsActive)
                .FirstOrDefaultAsync();

            if (account == null) return false;

            if (account.SubAccounts.Any(sa => sa.IsActive))
                throw new InvalidOperationException("Cannot delete an account that has active sub-accounts.");

            account.IsActive = false;
            account.ModifiedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
