using System;

namespace ERP.Features.JournalEntries.DTOs
{
    public class CreateJournalEntryLineDto
    {
        public Guid GlAccountId { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public Guid? TaxCodeId { get; set; }
        public Guid? DimensionId { get; set; }
    }
}
