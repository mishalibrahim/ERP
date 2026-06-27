using System;
using System.Collections.Generic;

namespace ERP.Features.JournalEntries.DTOs
{
    public class CreateJournalEntryDto
    {
        public string JournalName { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Currency { get; set; } = "AED";
        public string JournalType { get; set; } = "General";
        public string? CostCenter { get; set; }
        public string? Department { get; set; }
        public decimal ExchangeRate { get; set; } = 1.0m;
        public string Description { get; set; } = string.Empty;
        public List<CreateJournalEntryLineDto> Lines { get; set; } = new List<CreateJournalEntryLineDto>();
        public string? InternalNotes { get; set; }
    }
}
