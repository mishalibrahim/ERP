namespace ERP.Features.GeneralLedger.DTOs
{
    public class GlAccountOption
    {
        public string Id { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // "ASSETS" | "LIABILITIES" | "REVENUE" | "EXPENSES" | "EQUITY"
    }
}
