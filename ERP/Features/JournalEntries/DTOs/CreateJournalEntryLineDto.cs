using System;

namespace ERP.Features.JournalEntries.DTOs
{
    public class CreateJournalEntryLineDto
    {
        public string AccountType { get; set; } = "Ledger"; // Ledger, Customer, Vendor, Bank
        public Guid GlAccountId { get; set; }
        public string? Description { get; set; }
        public string? CostCenter { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public string? OffsetType { get; set; }
        public Guid? OffsetAccountId { get; set; }
    }
}
