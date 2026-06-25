using System;

namespace ERP.Features.JournalEntries.DTOs
{
    public class JournalEntryLineDto
    {
        public Guid Id { get; set; }
        public Guid GlAccountId { get; set; }
        public string GlAccountNumber { get; set; } = string.Empty;
        public string GlAccountName { get; set; } = string.Empty;
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public Guid? TaxCodeId { get; set; }
        public Guid? DimensionId { get; set; }
    }
}
