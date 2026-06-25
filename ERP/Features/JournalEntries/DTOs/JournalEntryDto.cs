using System;
using System.Collections.Generic;

namespace ERP.Features.JournalEntries.DTOs
{
    public class JournalEntryDto
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsPosted { get; set; }
        
        public List<JournalEntryLineDto> Lines { get; set; } = new List<JournalEntryLineDto>();
    }
}
