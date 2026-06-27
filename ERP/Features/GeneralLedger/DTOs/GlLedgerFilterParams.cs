using System;

namespace ERP.Features.GeneralLedger.DTOs
{
    public class GlLedgerFilterParams
    {
        public Guid? AccountId { get; set; }
        public string? PeriodValue { get; set; } // e.g. "2026-05", "YTD-2026", "FY-2026"
        public string? CostCenter { get; set; }
        public string? Type { get; set; }
    }
}
