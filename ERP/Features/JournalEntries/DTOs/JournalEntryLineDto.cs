using System;

namespace ERP.Features.JournalEntries.DTOs
{
    public class JournalEntryLineDto
    {
        public Guid Id { get; set; }
        public string AccountType { get; set; } = string.Empty; // "Ledger", "Customer", "Vendor", "Bank"
        public Guid GlAccountId { get; set; }
        public string GlAccountNumber { get; set; } = string.Empty;
        public string GlAccountName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? CostCenter { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        
        public string? OffsetType { get; set; } // "Ledger", "Bank"
        public Guid? OffsetAccountId { get; set; }
        public string? OffsetAccountNumber { get; set; }
        public string? OffsetAccountName { get; set; }
    }
}
