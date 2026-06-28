using System;

namespace ERP.Features.GeneralLedger.DTOs
{
    public class GlTransactionDto
    {
        public string Id { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string VoucherNo { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // JV, RV, PV, INV, PINV, OB
        public string Narration { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string AccountNumber { get; set; } = string.Empty;
        public string AccountName { get; set; } = string.Empty;
        public string CostCenter { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string PostedBy { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty; // Posted, Draft
    }
}
