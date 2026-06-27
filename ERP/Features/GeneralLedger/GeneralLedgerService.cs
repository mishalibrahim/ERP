using Erp.Module.GL.Data;
using Erp.Module.GL.Entities;
using Erp.Shared.Enums;
using ERP.Features.GeneralLedger.DTOs;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ERP.Features.GeneralLedger
{
    public class GeneralLedgerService : IGeneralLedgerService
    {
        private readonly GlDbContext _context;

        public GeneralLedgerService(GlDbContext context)
        {
            _context = context;
        }

        public async Task<List<GlTransactionDto>> GetTransactionsAsync(GlLedgerFilterParams filters)
        {
            var query = _context.JournalEntryLines
                .Include(l => l.GlAccount)
                .Include(l => l.JournalEntry)
                .Where(l => l.JournalEntry != null && l.JournalEntry.IsPosted)
                .AsQueryable();

            if (filters.AccountId.HasValue)
                query = query.Where(l => l.GlAccountId == filters.AccountId.Value);

            if (!string.IsNullOrEmpty(filters.PeriodValue))
            {
                var range = ParsePeriod(filters.PeriodValue);
                if (range.HasValue)
                    query = query.Where(l => l.JournalEntry!.Date >= range.Value.StartDate && l.JournalEntry.Date <= range.Value.EndDate);
            }

            if (!string.IsNullOrEmpty(filters.CostCenter) && filters.CostCenter != "all")
                query = query.Where(l => l.CostCenter == filters.CostCenter || (l.CostCenter == null && l.JournalEntry!.CostCenter == filters.CostCenter));

            if (!string.IsNullOrEmpty(filters.Type))
            {
                query = query.Where(l =>
                    (filters.Type == "JV" && (l.JournalEntry!.JournalType == JournalVoucherType.General || l.JournalEntry.VoucherNo.StartsWith("JV"))) ||
                    (filters.Type == "OB" && (l.JournalEntry!.JournalType == JournalVoucherType.Opening || l.JournalEntry.VoucherNo.StartsWith("JV"))) ||
                    l.JournalEntry!.VoucherNo.StartsWith(filters.Type)
                );
            }

            var lines = await query.ToListAsync();

            return lines.Select(l =>
            {
                var type = "JV";
                if (l.JournalEntry!.JournalType == JournalVoucherType.Opening) type = "OB";
                else if (l.JournalEntry.VoucherNo.StartsWith("RV-")) type = "RV";
                else if (l.JournalEntry.VoucherNo.StartsWith("PV-")) type = "PV";
                else if (l.JournalEntry.VoucherNo.StartsWith("INV-")) type = "INV";
                else if (l.JournalEntry.VoucherNo.StartsWith("PINV-")) type = "PINV";

                string postedBy = "System Admin";
                try
                {
                    if (!string.IsNullOrEmpty(l.JournalEntry.ApprovalHistoryJson))
                    {
                        var history = System.Text.Json.JsonSerializer.Deserialize<List<ApprovalHistoryItem>>(l.JournalEntry.ApprovalHistoryJson);
                        var postItem = history?.LastOrDefault(h => h.Action == "Approved" || h.Action == "Posted");
                        if (postItem != null) postedBy = postItem.Actor;
                    }
                }
                catch { }

                return new GlTransactionDto
                {
                    Id = l.Id.ToString(),
                    Date = l.JournalEntry.Date,
                    VoucherNo = l.JournalEntry.VoucherNo,
                    Type = type,
                    Narration = !string.IsNullOrEmpty(l.Description) ? l.Description : l.JournalEntry.Description,
                    AccountId = l.GlAccountId.ToString(),
                    AccountNumber = l.GlAccount?.AccountNumber ?? "",
                    AccountName = l.GlAccount?.AccountName ?? "",
                    CostCenter = !string.IsNullOrEmpty(l.CostCenter) ? l.CostCenter : (l.JournalEntry.CostCenter ?? "Admin"),
                    Debit = l.Debit,
                    Credit = l.Credit,
                    PostedBy = postedBy,
                    Status = "Posted"
                };
            })
            .OrderBy(t => t.Date)
            .ToList();
        }

        public async Task<decimal> GetOpeningBalanceAsync(Guid? accountId, string? periodValue)
        {
            if (accountId == null) return 0;

            var range = ParsePeriod(periodValue);
            var startDate = range?.StartDate ?? DateTime.MinValue;

            var openingDebit = await _context.JournalEntryLines
                .Where(l => l.GlAccountId == accountId && l.JournalEntry != null && l.JournalEntry.IsPosted && l.JournalEntry.Date < startDate)
                .SumAsync(l => l.Debit);

            var openingCredit = await _context.JournalEntryLines
                .Where(l => l.GlAccountId == accountId && l.JournalEntry != null && l.JournalEntry.IsPosted && l.JournalEntry.Date < startDate)
                .SumAsync(l => l.Credit);

            return openingDebit - openingCredit;
        }

        public async Task<decimal> GetClosingBalanceAsync(Guid? accountId, string? periodValue)
        {
            if (accountId == null) return 0;

            var opening = await GetOpeningBalanceAsync(accountId, periodValue);

            var range = ParsePeriod(periodValue);
            var query = _context.JournalEntryLines
                .Where(l => l.GlAccountId == accountId && l.JournalEntry != null && l.JournalEntry.IsPosted);

            if (range.HasValue)
                query = query.Where(l => l.JournalEntry!.Date >= range.Value.StartDate && l.JournalEntry.Date <= range.Value.EndDate);

            var periodDebit = await query.SumAsync(l => l.Debit);
            var periodCredit = await query.SumAsync(l => l.Credit);

            return opening + periodDebit - periodCredit;
        }

        public Task<List<GlPeriodOption>> GetPeriodsAsync()
        {
            var options = new List<GlPeriodOption>
            {
                new() { Label = "Jun 2026", Value = "2026-06", StartDate = "2026-06-01", EndDate = "2026-06-30" },
                new() { Label = "May 2026", Value = "2026-05", StartDate = "2026-05-01", EndDate = "2026-05-31" },
                new() { Label = "Apr 2026", Value = "2026-04", StartDate = "2026-04-01", EndDate = "2026-04-30" },
                new() { Label = "Mar 2026", Value = "2026-03", StartDate = "2026-03-01", EndDate = "2026-03-31" },
                new() { Label = "Feb 2026", Value = "2026-02", StartDate = "2026-02-01", EndDate = "2026-02-28" },
                new() { Label = "Jan 2026", Value = "2026-01", StartDate = "2026-01-01", EndDate = "2026-01-31" },
                new() { Label = "YTD 2026 (Jul-May)", Value = "YTD-2026", StartDate = "2025-07-01", EndDate = "2026-05-31" },
                new() { Label = "Full Year 2026", Value = "FY-2026", StartDate = "2025-07-01", EndDate = "2026-06-30" }
            };
            return Task.FromResult(options);
        }

        public async Task<List<GlCostCenterOption>> GetCostCentersAsync()
        {
            var dims = await _context.Dimensions
                .Where(d => d.Type == DimensionType.CostCenter && d.IsActive)
                .ToListAsync();

            var options = new List<GlCostCenterOption> { new() { Label = "All Cost Centers", Value = "all" } };
            foreach (var cc in dims)
                options.Add(new GlCostCenterOption { Label = cc.Name, Value = cc.Code });

            return options;
        }

        public async Task<List<GlAccountOption>> GetAccountsAsync()
        {
            var accounts = await _context.GlAccounts
                .Where(a => a.PostingType == GlPostingType.Posting && a.IsActive)
                .OrderBy(a => a.AccountNumber)
                .ToListAsync();

            return accounts.Select(a => new GlAccountOption
            {
                Id = a.Id.ToString(),
                AccountNumber = a.AccountNumber,
                AccountName = a.AccountName,
                Category = a.AccountCategory switch
                {
                    GlAccountCategory.Asset => "ASSETS",
                    GlAccountCategory.Liability => "LIABILITIES",
                    GlAccountCategory.Equity => "EQUITY",
                    GlAccountCategory.Income => "REVENUE",
                    GlAccountCategory.Expense => "EXPENSES",
                    _ => "ASSETS"
                }
            }).ToList();
        }

        // --- Trial Balance ---

        public async Task<TrialBalanceSummaryDto> GetTrialBalanceAsync(string? periodValue)
        {
            var range = ParsePeriod(periodValue);
            var endDate = range?.EndDate ?? DateTime.UtcNow.Date;
            var startDate = range?.StartDate ?? DateTime.MinValue;

            var accounts = await _context.GlAccounts
                .Where(a => a.PostingType == GlPostingType.Posting && a.IsActive)
                .OrderBy(a => a.AccountNumber)
                .ToListAsync();

            var allLines = await _context.JournalEntryLines
                .Where(l => l.JournalEntry != null && l.JournalEntry.IsPosted && l.JournalEntry.Date <= endDate)
                .Select(l => new { l.GlAccountId, l.Debit, l.Credit, Date = l.JournalEntry!.Date })
                .ToListAsync();

            var lines = new List<TrialBalanceDto>();

            foreach (var account in accounts)
            {
                var acctLines = allLines.Where(l => l.GlAccountId == account.Id).ToList();

                var openingDebit = acctLines.Where(l => l.Date < startDate).Sum(l => l.Debit);
                var openingCredit = acctLines.Where(l => l.Date < startDate).Sum(l => l.Credit);
                var periodDebit = acctLines.Where(l => l.Date >= startDate && l.Date <= endDate).Sum(l => l.Debit);
                var periodCredit = acctLines.Where(l => l.Date >= startDate && l.Date <= endDate).Sum(l => l.Credit);

                if (openingDebit == 0 && openingCredit == 0 && periodDebit == 0 && periodCredit == 0)
                    continue;

                var closingNet = (openingDebit - openingCredit) + (periodDebit - periodCredit);

                lines.Add(new TrialBalanceDto
                {
                    AccountNumber = account.AccountNumber,
                    AccountName = account.AccountName,
                    Category = account.AccountCategory switch
                    {
                        GlAccountCategory.Asset => "ASSETS",
                        GlAccountCategory.Liability => "LIABILITIES",
                        GlAccountCategory.Equity => "EQUITY",
                        GlAccountCategory.Income => "REVENUE",
                        GlAccountCategory.Expense => "EXPENSES",
                        _ => "ASSETS"
                    },
                    OpeningDebit = openingDebit,
                    OpeningCredit = openingCredit,
                    PeriodDebit = periodDebit,
                    PeriodCredit = periodCredit,
                    ClosingDebit = closingNet >= 0 ? closingNet : 0,
                    ClosingCredit = closingNet < 0 ? Math.Abs(closingNet) : 0
                });
            }

            var periods = await GetPeriodsAsync();
            var label = periods.FirstOrDefault(p => p.Value == periodValue)?.Label ?? periodValue ?? "All Periods";

            return new TrialBalanceSummaryDto
            {
                PeriodValue = periodValue ?? "",
                PeriodLabel = label,
                Lines = lines
            };
        }

        // --- CSV Export ---

        public async Task<byte[]> ExportTransactionsCsvAsync(GlLedgerFilterParams filters)
        {
            var rows = await GetTransactionsAsync(filters);

            var sb = new StringBuilder();
            sb.AppendLine("\"Aegis ERP - General Ledger Export\"");
            sb.AppendLine($"\"Period: {filters.PeriodValue ?? "All"}\"");
            sb.AppendLine($"\"Exported: {DateTime.UtcNow:dd MMM yyyy HH:mm} UTC\"");
            sb.AppendLine();
            sb.AppendLine("\"Date\",\"Voucher No.\",\"Type\",\"Narration\",\"Account Number\",\"Account Name\",\"Cost Center\",\"Debit (AED)\",\"Credit (AED)\",\"Posted By\",\"Status\"");

            decimal totalDebit = 0, totalCredit = 0;
            foreach (var r in rows)
            {
                totalDebit += r.Debit;
                totalCredit += r.Credit;
                sb.AppendLine($"\"{r.Date:dd MMM yyyy}\",\"{r.VoucherNo}\",\"{r.Type}\",\"{r.Narration}\",\"{r.AccountNumber}\",\"{r.AccountName}\",\"{r.CostCenter}\",\"{r.Debit:F2}\",\"{r.Credit:F2}\",\"{r.PostedBy}\",\"{r.Status}\"");
            }

            sb.AppendLine();
            sb.AppendLine($"\"TOTAL\",\"\",\"\",\"\",\"\",\"\",\"\",\"{totalDebit:F2}\",\"{totalCredit:F2}\",\"\",\"\"");

            return Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(sb.ToString())).ToArray();
        }

        // --- Period Locking ---

        public async Task<List<PeriodLockDto>> GetPeriodLocksAsync()
        {
            var periods = await GetPeriodsAsync();
            var locks = await _context.LockedPeriods.ToListAsync();

            return periods
                .Where(p => p.Value.Length == 7 && p.Value[4] == '-')
                .Select(p =>
                {
                    var parts = p.Value.Split('-');
                    int year = int.Parse(parts[0]);
                    int month = int.Parse(parts[1]);
                    var lockEntry = locks.FirstOrDefault(l => l.Year == year && l.Month == month);
                    return new PeriodLockDto
                    {
                        PeriodValue = p.Value,
                        PeriodLabel = p.Label,
                        IsLocked = lockEntry?.IsLocked ?? false,
                        LockedBy = lockEntry?.LockedBy,
                        LockedAt = lockEntry?.LockedAt
                    };
                })
                .ToList();
        }

        public async Task<PeriodLockDto> SetPeriodLockAsync(string periodValue, bool isLocked)
        {
            var parts = periodValue.Split('-');
            if (parts.Length != 2 || !int.TryParse(parts[0], out int year) || !int.TryParse(parts[1], out int month))
                throw new InvalidOperationException($"Invalid period value: {periodValue}.");

            var existing = await _context.LockedPeriods
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Year == year && l.Month == month);

            var periods = await GetPeriodsAsync();
            var label = periods.FirstOrDefault(p => p.Value == periodValue)?.Label ?? periodValue;

            if (existing == null)
            {
                var newLock = new LockedPeriod
                {
                    Year = year,
                    Month = month,
                    IsLocked = isLocked,
                    LockedBy = isLocked ? "Finance Manager" : null,
                    LockedAt = isLocked ? DateTime.UtcNow : null
                };
                _context.LockedPeriods.Add(newLock);
                await _context.SaveChangesAsync();
                return new PeriodLockDto { PeriodValue = periodValue, PeriodLabel = label, IsLocked = isLocked, LockedBy = newLock.LockedBy, LockedAt = newLock.LockedAt };
            }

            existing.IsLocked = isLocked;
            existing.LockedBy = isLocked ? "Finance Manager" : null;
            existing.LockedAt = isLocked ? DateTime.UtcNow : null;
            await _context.SaveChangesAsync();

            return new PeriodLockDto { PeriodValue = periodValue, PeriodLabel = label, IsLocked = isLocked, LockedBy = existing.LockedBy, LockedAt = existing.LockedAt };
        }

        public async Task<bool> IsPeriodLockedAsync(DateTime date)
        {
            return await _context.LockedPeriods
                .AnyAsync(l => l.Year == date.Year && l.Month == date.Month && l.IsLocked);
        }

        // --- Helpers ---

        private (DateTime StartDate, DateTime EndDate)? ParsePeriod(string? periodValue)
        {
            if (string.IsNullOrEmpty(periodValue)) return null;

            if (periodValue.StartsWith("YTD-"))
            {
                if (int.TryParse(periodValue.Substring(4), out int year))
                    return (new DateTime(year - 1, 7, 1), new DateTime(year, 5, 31));
            }
            if (periodValue.StartsWith("FY-"))
            {
                if (int.TryParse(periodValue.Substring(3), out int year))
                    return (new DateTime(year - 1, 7, 1), new DateTime(year, 6, 30));
            }

            var parts = periodValue.Split('-');
            if (parts.Length == 2 && int.TryParse(parts[0], out int y) && int.TryParse(parts[1], out int m))
            {
                var start = new DateTime(y, m, 1);
                var end = start.AddMonths(1).AddDays(-1);
                return (start, end);
            }

            return null;
        }

        private class ApprovalHistoryItem
        {
            public string Stage { get; set; } = string.Empty;
            public string Action { get; set; } = string.Empty;
            public string Actor { get; set; } = string.Empty;
            public string Timestamp { get; set; } = string.Empty;
            public string? Remarks { get; set; }
        }
    }
}
