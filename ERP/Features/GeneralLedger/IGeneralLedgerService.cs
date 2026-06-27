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
        Task<List<GlPeriodOption>> GetPeriodsAsync();
        Task<List<GlCostCenterOption>> GetCostCentersAsync();
        Task<List<GlAccountOption>> GetAccountsAsync();
    }
}
