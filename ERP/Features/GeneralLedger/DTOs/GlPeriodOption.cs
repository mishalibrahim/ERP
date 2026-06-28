using System;

namespace ERP.Features.GeneralLedger.DTOs
{
    public class GlPeriodOption
    {
        public string Label { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty; // "2026-05", "YTD-2026", "FY-2026"
        public string StartDate { get; set; } = string.Empty; // ISO date format string "YYYY-MM-DD"
        public string EndDate { get; set; } = string.Empty; // ISO date format string "YYYY-MM-DD"
    }
}
