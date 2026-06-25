using System;
using System.Collections.Generic;

namespace ERP.Features.JournalEntries.DTOs
{
    public class CreateJournalEntryDto
    {
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public bool PostImmediately { get; set; }
        
        public List<CreateJournalEntryLineDto> Lines { get; set; } = new List<CreateJournalEntryLineDto>();
    }
}
