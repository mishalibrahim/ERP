using ERP.Features.GeneralLedger.DTOs;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ERP.Features.GeneralLedger
{
    public interface IGeneralLedgerService
    {
        Task<List<GlTransactionDto>> GetTransactionsAsync(GlLedgerFilterParams filters);
        Task<decimal> GetOpeningBalanceAsync(Guid? accountId, string? periodValue);
        Task<decimal> GetClosingBalanceAsync(Guid? accountId, string? periodValue);
        Task<List<GlPeriodOption>> GetPeriodsAsync();
        Task<List<GlCostCenterOption>> GetCostCentersAsync();
        Task<List<GlAccountOption>> GetAccountsAsync();

        // Trial Balance
        Task<TrialBalanceSummaryDto> GetTrialBalanceAsync(string? periodValue);

        // CSV Export
        Task<byte[]> ExportTransactionsCsvAsync(GlLedgerFilterParams filters);

        // Period Locking
        Task<List<PeriodLockDto>> GetPeriodLocksAsync();
        Task<PeriodLockDto> SetPeriodLockAsync(string periodValue, bool isLocked);
        Task<bool> IsPeriodLockedAsync(DateTime date);
    }
}
