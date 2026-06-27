namespace ERP.Features.GeneralLedger.DTOs
{
    public class TrialBalanceDto
    {
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // ASSETS, LIABILITIES, EQUITY, REVENUE, EXPENSES

        public decimal OpeningDebit { get; set; }
        public decimal OpeningCredit { get; set; }

        public decimal PeriodDebit { get; set; }
        public decimal PeriodCredit { get; set; }

        public decimal ClosingDebit { get; set; }
        public decimal ClosingCredit { get; set; }
    }

    public class TrialBalanceSummaryDto
    {
        public string PeriodValue { get; set; } = string.Empty;
        public string PeriodLabel { get; set; } = string.Empty;
        public List<TrialBalanceDto> Lines { get; set; } = new();

        public decimal TotalOpeningDebit => Lines.Sum(l => l.OpeningDebit);
        public decimal TotalOpeningCredit => Lines.Sum(l => l.OpeningCredit);
        public decimal TotalPeriodDebit => Lines.Sum(l => l.PeriodDebit);
        public decimal TotalPeriodCredit => Lines.Sum(l => l.PeriodCredit);
        public decimal TotalClosingDebit => Lines.Sum(l => l.ClosingDebit);
        public decimal TotalClosingCredit => Lines.Sum(l => l.ClosingCredit);
    }
}
