using Erp.Module.GL.Data;
using Erp.Module.GL.Entities;
using ERP.Features.JournalEntries.DTOs;
using Erp.Shared.Enums;
using Erp.Shared.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

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
                .Include(j => j.Lines)
                .ThenInclude(l => l.OffsetAccount)
                .OrderByDescending(j => j.Date)
                .ToListAsync();

            return entries.Select(MapToDto).ToList();
        }

        public async Task<JournalEntryDto?> GetByIdAsync(Guid id)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.Lines)
                .ThenInclude(l => l.GlAccount)
                .Include(j => j.Lines)
                .ThenInclude(l => l.OffsetAccount)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) return null;

            return MapToDto(entry);
        }

        public async Task<JournalEntryDto> SaveAsync(CreateJournalEntryDto dto, Guid? id)
        {
            var tenantId = _currentUserService.TenantId;
            if (tenantId == null) throw new UnauthorizedAccessException("Tenant ID is required.");

            JournalEntry entry;

            if (id.HasValue && id.Value != Guid.Empty)
            {
                // Update existing draft
                entry = await _context.JournalEntries
                    .Include(j => j.Lines)
                    .FirstOrDefaultAsync(j => j.Id == id.Value);

                if (entry == null)
                    throw new InvalidOperationException("Voucher not found.");

                if (entry.Status != JournalVoucherStatus.Draft && entry.Status != JournalVoucherStatus.Rejected)
                    throw new InvalidOperationException("Only draft or rejected vouchers can be updated.");

                // Map updated fields
                entry.JournalName = dto.JournalName;
                entry.Date = dto.Date;
                entry.Currency = dto.Currency;
                entry.JournalType = ParseType(dto.JournalType);
                entry.CostCenter = dto.CostCenter;
                entry.Department = dto.Department;
                entry.ExchangeRate = dto.ExchangeRate;
                entry.Description = dto.Description;
                entry.InternalNotes = dto.InternalNotes;
                entry.Status = JournalVoucherStatus.Draft; // reset rejected status to draft on edit

                // Remove existing lines and recreate them
                _context.JournalEntryLines.RemoveRange(entry.Lines);
                entry.Lines.Clear();
            }
            else
            {
                // Create new voucher
                var voucherNo = await GenerateVoucherNoAsync(dto.Date);
                entry = new JournalEntry
                {
                    TenantId = tenantId.Value,
                    VoucherNo = voucherNo,
                    JournalName = dto.JournalName,
                    Date = dto.Date,
                    Currency = dto.Currency,
                    JournalType = ParseType(dto.JournalType),
                    CostCenter = dto.CostCenter,
                    Department = dto.Department,
                    ExchangeRate = dto.ExchangeRate,
                    Description = dto.Description,
                    InternalNotes = dto.InternalNotes,
                    Status = JournalVoucherStatus.Draft,
                    CurrentApprovalStage = JournalVoucherApprovalStage.Initiator,
                    ApprovalHistoryJson = "[]",
                    AttachmentsJson = "[]"
                };
                _context.JournalEntries.Add(entry);
            }

            // Map and add new lines
            foreach (var lineDto in dto.Lines)
            {
                entry.Lines.Add(new JournalEntryLine
                {
                    TenantId = tenantId.Value,
                    AccountType = ParseAccountType(lineDto.AccountType),
                    GlAccountId = lineDto.GlAccountId,
                    Description = lineDto.Description,
                    CostCenter = lineDto.CostCenter,
                    Debit = lineDto.Debit,
                    Credit = lineDto.Credit,
                    OffsetType = !string.IsNullOrEmpty(lineDto.OffsetType) ? ParseAccountType(lineDto.OffsetType) : null,
                    OffsetAccountId = lineDto.OffsetAccountId
                });
            }

            await _context.SaveChangesAsync();
            return MapToDto(entry);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) return false;

            if (entry.Status != JournalVoucherStatus.Draft && entry.Status != JournalVoucherStatus.Rejected)
                throw new InvalidOperationException("Only draft or rejected vouchers can be deleted.");

            entry.IsActive = false;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<(bool Success, List<string> Errors)> ValidateVoucherAsync(Guid id)
        {
            var errors = new List<string>();
            var entry = await _context.JournalEntries
                .Include(j => j.Lines)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null)
            {
                errors.Add("Voucher not found.");
                return (false, errors);
            }

            // 1. Basic balancing
            decimal totalDebit = entry.Lines.Sum(l => l.Debit);
            decimal totalCredit = entry.Lines.Sum(l => l.Credit);
            if (Math.Abs(totalDebit - totalCredit) >= 0.01m)
            {
                errors.Add($"Journal is unbalanced: Total Debits (AED {(totalDebit * entry.ExchangeRate):F2}) must equal Total Credits (AED {(totalCredit * entry.ExchangeRate):F2}).");
            }

            // 2. Open Period Check (Jan 2026 - Dec 2026 open)
            if (entry.Date.Year != 2026)
            {
                errors.Add($"Voucher Date {entry.Date:yyyy-MM-dd} falls outside the open fiscal period (Year 2026).");
            }

            // 3. Line count check
            if (entry.Lines.Count < 2)
            {
                errors.Add("At least 2 lines are required for a double-entry posting.");
            }

            // 4. Line field selections
            int index = 1;
            foreach (var l in entry.Lines)
            {
                if (l.GlAccountId == Guid.Empty)
                {
                    errors.Add($"Line {index}: Account selection is mandatory.");
                }
                if (l.Debit == 0 && l.Credit == 0)
                {
                    errors.Add($"Line {index}: Must specify a Debit or Credit amount.");
                }
                if (l.Debit > 0 && l.Credit > 0)
                {
                    errors.Add($"Line {index}: Cannot have both a Debit and a Credit amount.");
                }
                index++;
            }

            // 5. Exchange Rate check
            if (entry.Currency != "AED" && entry.ExchangeRate <= 0)
            {
                errors.Add("Exchange Rate must be greater than zero for foreign currencies.");
            }

            return (errors.Count == 0, errors);
        }

        public async Task<List<object>> SimulateVoucherAsync(Guid id)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.Lines)
                .ThenInclude(l => l.GlAccount)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) throw new InvalidOperationException("Voucher not found.");

            var simulation = new List<object>();
            foreach (var l in entry.Lines)
            {
                simulation.Add(new
                {
                    accountId = l.GlAccountId,
                    accountNumber = l.GlAccount?.AccountNumber ?? "",
                    accountName = l.GlAccount?.AccountName ?? "",
                    costCenter = l.CostCenter ?? entry.CostCenter ?? "Admin",
                    debit = l.Debit * entry.ExchangeRate, // base currency value
                    credit = l.Credit * entry.ExchangeRate, // base currency value
                    currency = "AED",
                    foreignDebit = entry.Currency != "AED" ? l.Debit : 0m,
                    foreignCredit = entry.Currency != "AED" ? l.Credit : 0m,
                    foreignCurrency = entry.Currency
                });
            }

            return simulation;
        }

        public async Task<JournalEntryDto> SendForApprovalAsync(Guid id)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.Lines)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) throw new InvalidOperationException("Voucher not found.");

            if (entry.Status != JournalVoucherStatus.Draft && entry.Status != JournalVoucherStatus.Rejected)
                throw new InvalidOperationException("Voucher must be in Draft or Rejected status to send for approval.");

            // Run validation check
            var (valid, errors) = await ValidateVoucherAsync(id);
            if (!valid)
            {
                throw new InvalidOperationException("Validation failed: " + string.Join(" ", errors));
            }

            entry.Status = JournalVoucherStatus.PendingApproval;
            entry.CurrentApprovalStage = JournalVoucherApprovalStage.FinanceReview;

            var history = DeserializeHistory(entry.ApprovalHistoryJson);
            history.Add(new JournalVoucherApprovalHistoryItem
            {
                Stage = "Initiator",
                Action = "Submitted",
                Actor = GetCurrentUserName(),
                Timestamp = DateTime.UtcNow
            });
            entry.ApprovalHistoryJson = JsonSerializer.Serialize(history);

            await _context.SaveChangesAsync();
            return MapToDto(entry);
        }

        public async Task<JournalEntryDto> ApproveAsync(Guid id, string? remarks)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.Lines)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) throw new InvalidOperationException("Voucher not found.");

            if (entry.Status != JournalVoucherStatus.PendingApproval)
                throw new InvalidOperationException("Voucher is not pending approval.");

            var currentStage = entry.CurrentApprovalStage;
            JournalVoucherApprovalStage nextStage;
            JournalVoucherStatus nextStatus = JournalVoucherStatus.PendingApproval;

            // CFO threshold calculation: sum total debit in AED
            decimal totalDebitBase = entry.Lines.Sum(l => l.Debit) * entry.ExchangeRate;

            var history = DeserializeHistory(entry.ApprovalHistoryJson);

            if (currentStage == JournalVoucherApprovalStage.FinanceReview)
            {
                if (totalDebitBase <= 50000m)
                {
                    nextStage = JournalVoucherApprovalStage.Posted; // Bypasses CFO
                    nextStatus = JournalVoucherStatus.Approved;
                }
                else
                {
                    nextStage = JournalVoucherApprovalStage.CfoApprove;
                    nextStatus = JournalVoucherStatus.PendingApproval;
                }

                history.Add(new JournalVoucherApprovalHistoryItem
                {
                    Stage = "Finance Review",
                    Action = "Approved",
                    Actor = "Sara Al-Rashid", // Mock Finance User
                    Timestamp = DateTime.UtcNow,
                    Remarks = remarks ?? "Validated lines, cost centers and balance check."
                });
            }
            else if (currentStage == JournalVoucherApprovalStage.CfoApprove)
            {
                nextStage = JournalVoucherApprovalStage.Posted;
                nextStatus = JournalVoucherStatus.Approved;

                history.Add(new JournalVoucherApprovalHistoryItem
                {
                    Stage = "CFO Approve",
                    Action = "Approved",
                    Actor = "Layla Hassan", // Mock CFO User
                    Timestamp = DateTime.UtcNow,
                    Remarks = remarks ?? "Approved."
                });
            }
            else
            {
                throw new InvalidOperationException("Invalid approval stage transition.");
            }

            entry.CurrentApprovalStage = nextStage;
            entry.Status = nextStatus;
            entry.ApprovalHistoryJson = JsonSerializer.Serialize(history);

            await _context.SaveChangesAsync();
            return MapToDto(entry);
        }

        public async Task<JournalEntryDto> RejectAsync(Guid id, string? remarks)
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) throw new InvalidOperationException("Voucher not found.");

            if (entry.Status != JournalVoucherStatus.PendingApproval)
                throw new InvalidOperationException("Voucher is not pending approval.");

            var currentStage = entry.CurrentApprovalStage;
            entry.Status = JournalVoucherStatus.Rejected;
            entry.CurrentApprovalStage = JournalVoucherApprovalStage.Initiator;
            entry.ApprovalRemarks = remarks ?? "";

            var history = DeserializeHistory(entry.ApprovalHistoryJson);
            history.Add(new JournalVoucherApprovalHistoryItem
            {
                Stage = currentStage.ToString(),
                Action = "Rejected",
                Actor = currentStage == JournalVoucherApprovalStage.FinanceReview ? "Sara Al-Rashid" : "Layla Hassan",
                Timestamp = DateTime.UtcNow,
                Remarks = remarks ?? "Rejected."
            });
            entry.ApprovalHistoryJson = JsonSerializer.Serialize(history);

            await _context.SaveChangesAsync();
            return MapToDto(entry);
        }

        public async Task<JournalEntryDto> PostVoucherAsync(Guid id)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.Lines)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) throw new InvalidOperationException("Voucher not found.");

            if (entry.Status != JournalVoucherStatus.Approved)
                throw new InvalidOperationException("Voucher must be Approved before posting.");

            entry.Status = JournalVoucherStatus.Posted;
            entry.IsPosted = true; // Sync base field
            entry.CurrentApprovalStage = JournalVoucherApprovalStage.Posted;

            var history = DeserializeHistory(entry.ApprovalHistoryJson);
            history.Add(new JournalVoucherApprovalHistoryItem
            {
                Stage = "Posted",
                Action = "Posted",
                Actor = GetCurrentUserName(),
                Timestamp = DateTime.UtcNow
            });
            entry.ApprovalHistoryJson = JsonSerializer.Serialize(history);

            // Reversing Voucher Automation
            if (entry.JournalType == JournalVoucherType.Reversing)
            {
                var nextVoucherNo = await GenerateVoucherNoAsync(entry.Date.AddMonths(1));
                
                // Swap debits and credits and set date to 1st of next month
                var nextMonthDate = new DateTime(entry.Date.Year, entry.Date.Month, 1).AddMonths(1);

                var reversingVoucher = new JournalEntry
                {
                    TenantId = entry.TenantId,
                    VoucherNo = nextVoucherNo,
                    JournalName = entry.JournalName,
                    Date = nextMonthDate,
                    Currency = entry.Currency,
                    JournalType = JournalVoucherType.General, // Reversal itself is a standard entry
                    CostCenter = entry.CostCenter,
                    Department = entry.Department,
                    ExchangeRate = entry.ExchangeRate,
                    Description = $"Auto-reversal of voucher {entry.VoucherNo}",
                    Status = JournalVoucherStatus.Draft,
                    CurrentApprovalStage = JournalVoucherApprovalStage.Initiator,
                    InternalNotes = $"Auto-generated reversing entry for accrual voucher {entry.VoucherNo}.",
                    ReversedVoucherId = entry.Id,
                    ApprovalHistoryJson = "[]",
                    AttachmentsJson = "[]"
                };

                foreach (var line in entry.Lines)
                {
                    reversingVoucher.Lines.Add(new JournalEntryLine
                    {
                        TenantId = entry.TenantId,
                        AccountType = line.AccountType,
                        GlAccountId = line.GlAccountId,
                        Description = $"Reversal of {entry.VoucherNo} - {line.Description}",
                        CostCenter = line.CostCenter,
                        Debit = line.Credit, // Swapped
                        Credit = line.Debit, // Swapped
                        OffsetType = line.OffsetType,
                        OffsetAccountId = line.OffsetAccountId
                    });
                }

                _context.JournalEntries.Add(reversingVoucher);
                await _context.SaveChangesAsync();

                entry.ReversingVoucherId = reversingVoucher.Id;
            }

            await _context.SaveChangesAsync();
            return MapToDto(entry);
        }

        public async Task<JournalEntryDto> ReverseVoucherAsync(Guid id)
        {
            var entry = await _context.JournalEntries
                .Include(j => j.Lines)
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) throw new InvalidOperationException("Voucher not found.");

            if (entry.Status != JournalVoucherStatus.Posted)
                throw new InvalidOperationException("Only posted vouchers can be reversed.");

            entry.Status = JournalVoucherStatus.Reversed;

            var nextVoucherNo = await GenerateVoucherNoAsync(DateTime.UtcNow);

            var reversingVoucher = new JournalEntry
            {
                TenantId = entry.TenantId,
                VoucherNo = nextVoucherNo,
                JournalName = entry.JournalName,
                Date = DateTime.UtcNow.Date,
                Currency = entry.Currency,
                JournalType = JournalVoucherType.General,
                CostCenter = entry.CostCenter,
                Department = entry.Department,
                ExchangeRate = entry.ExchangeRate,
                Description = $"Manual Reversal of voucher {entry.VoucherNo}",
                Status = JournalVoucherStatus.Draft,
                CurrentApprovalStage = JournalVoucherApprovalStage.Initiator,
                InternalNotes = $"Manually generated reversal for voucher {entry.VoucherNo}.",
                ReversedVoucherId = entry.Id,
                ApprovalHistoryJson = "[]",
                AttachmentsJson = "[]"
            };

            foreach (var line in entry.Lines)
            {
                reversingVoucher.Lines.Add(new JournalEntryLine
                {
                    TenantId = entry.TenantId,
                    AccountType = line.AccountType,
                    GlAccountId = line.GlAccountId,
                    Description = $"Reversal of {entry.VoucherNo} - {line.Description}",
                    CostCenter = line.CostCenter,
                    Debit = line.Credit,
                    Credit = line.Debit,
                    OffsetType = line.OffsetType,
                    OffsetAccountId = line.OffsetAccountId
                });
            }

            _context.JournalEntries.Add(reversingVoucher);
            await _context.SaveChangesAsync();

            entry.ReversingVoucherId = reversingVoucher.Id;
            await _context.SaveChangesAsync();

            return MapToDto(reversingVoucher);
        }

        public async Task<JournalEntryDto> AddAttachmentAsync(Guid id, JournalVoucherAttachment attachment)
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) throw new InvalidOperationException("Voucher not found.");

            var attachments = DeserializeAttachments(entry.AttachmentsJson);
            attachments.Add(attachment);
            entry.AttachmentsJson = JsonSerializer.Serialize(attachments);

            await _context.SaveChangesAsync();
            return MapToDto(entry);
        }

        public async Task<JournalEntryDto> RemoveAttachmentAsync(Guid id, string fileName)
        {
            var entry = await _context.JournalEntries
                .FirstOrDefaultAsync(j => j.Id == id);

            if (entry == null) throw new InvalidOperationException("Voucher not found.");

            var attachments = DeserializeAttachments(entry.AttachmentsJson);
            attachments = attachments.Where(a => a.Name != fileName).ToList();
            entry.AttachmentsJson = JsonSerializer.Serialize(attachments);

            await _context.SaveChangesAsync();
            return MapToDto(entry);
        }

        // Helper maps
        private JournalEntryDto MapToDto(JournalEntry entry)
        {
            return new JournalEntryDto
            {
                Id = entry.Id,
                VoucherNo = entry.VoucherNo,
                JournalName = entry.JournalName,
                Date = entry.Date,
                Currency = entry.Currency,
                JournalType = MapTypeToString(entry.JournalType),
                CostCenter = entry.CostCenter,
                Department = entry.Department,
                ExchangeRate = entry.ExchangeRate,
                Description = entry.Description,
                Status = MapStatusToString(entry.Status),
                InternalNotes = entry.InternalNotes ?? string.Empty,
                ApprovalRemarks = entry.ApprovalRemarks ?? string.Empty,
                CurrentApprovalStage = MapStageToString(entry.CurrentApprovalStage),
                ReversedVoucherId = entry.ReversedVoucherId,
                ReversingVoucherId = entry.ReversingVoucherId,
                Attachments = DeserializeAttachments(entry.AttachmentsJson),
                ApprovalHistory = DeserializeHistory(entry.ApprovalHistoryJson),
                Lines = entry.Lines.Select(l => new JournalEntryLineDto
                {
                    Id = l.Id,
                    AccountType = l.AccountType.ToString(),
                    GlAccountId = l.GlAccountId,
                    GlAccountNumber = l.GlAccount?.AccountNumber ?? "",
                    GlAccountName = l.GlAccount?.AccountName ?? "",
                    Description = l.Description,
                    CostCenter = l.CostCenter,
                    Debit = l.Debit,
                    Credit = l.Credit,
                    OffsetType = l.OffsetType?.ToString(),
                    OffsetAccountId = l.OffsetAccountId,
                    OffsetAccountNumber = l.OffsetAccount?.AccountNumber,
                    OffsetAccountName = l.OffsetAccount?.AccountName
                }).ToList()
            };
        }

        private async Task<string> GenerateVoucherNoAsync(DateTime date)
        {
            int year = date.Year;
            var prefix = $"JV-{year}-";
            
            var maxSeq = await _context.JournalEntries
                .Where(j => j.VoucherNo.StartsWith(prefix))
                .Select(j => j.VoucherNo)
                .ToListAsync();

            int nextNum = 1;
            if (maxSeq.Count > 0)
            {
                var nums = maxSeq.Select(v => {
                    var parts = v.Split('-');
                    if (parts.Length > 2 && int.TryParse(parts[2], out int num))
                    {
                        return num;
                    }
                    return 0;
                });
                nextNum = nums.Max() + 1;
            }

            return $"JV-{year}-{nextNum:D4}";
        }

        private string GetCurrentUserName()
        {
            // Fallback to Mishal Admin if not resolved
            return "Ahmed Khalil";
        }

        // Serialization
        private List<JournalVoucherAttachment> DeserializeAttachments(string json)
        {
            if (string.IsNullOrEmpty(json)) return new List<JournalVoucherAttachment>();
            try
            {
                return JsonSerializer.Deserialize<List<JournalVoucherAttachment>>(json) ?? new List<JournalVoucherAttachment>();
            }
            catch
            {
                return new List<JournalVoucherAttachment>();
            }
        }

        private List<JournalVoucherApprovalHistoryItem> DeserializeHistory(string json)
        {
            if (string.IsNullOrEmpty(json)) return new List<JournalVoucherApprovalHistoryItem>();
            try
            {
                return JsonSerializer.Deserialize<List<JournalVoucherApprovalHistoryItem>>(json) ?? new List<JournalVoucherApprovalHistoryItem>();
            }
            catch
            {
                return new List<JournalVoucherApprovalHistoryItem>();
            }
        }

        // String formatting helpers for UI mapping
        private string MapStatusToString(JournalVoucherStatus status) => status switch
        {
            JournalVoucherStatus.Draft => "Draft",
            JournalVoucherStatus.PendingApproval => "Pending Approval",
            JournalVoucherStatus.Approved => "Approved",
            JournalVoucherStatus.Posted => "Posted",
            JournalVoucherStatus.Rejected => "Rejected",
            JournalVoucherStatus.Reversed => "Reversed",
            _ => status.ToString()
        };

        private string MapTypeToString(JournalVoucherType type) => type.ToString();

        private string MapStageToString(JournalVoucherApprovalStage stage) => stage switch
        {
            JournalVoucherApprovalStage.Initiator => "Initiator",
            JournalVoucherApprovalStage.FinanceReview => "Finance Review",
            JournalVoucherApprovalStage.CfoApprove => "CFO Approve",
            JournalVoucherApprovalStage.Posted => "Posted",
            _ => stage.ToString()
        };

        private JournalVoucherType ParseType(string type) => type switch
        {
            "General" => JournalVoucherType.General,
            "Adjusting" => JournalVoucherType.Adjusting,
            "Accrual" => JournalVoucherType.Accrual,
            "Reversing" => JournalVoucherType.Reversing,
            "Opening" => JournalVoucherType.Opening,
            _ => Enum.Parse<JournalVoucherType>(type)
        };

        private GlAccountType ParseAccountType(string type) => type switch
        {
            "Ledger" => GlAccountType.Ledger,
            "Customer" => GlAccountType.Customer,
            "Vendor" => GlAccountType.Vendor,
            "Bank" => GlAccountType.Bank,
            _ => Enum.Parse<GlAccountType>(type)
        };
    }
}
