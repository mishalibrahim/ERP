using System;
using System.Collections.Generic;
using Erp.Shared.Entities;

namespace Erp.Module.GL.Entities
{
    public class JournalEntry : BaseEntity
    {
        public Guid TenantId { get; set; }
        
        public DateTime Date { get; set; }
        public string Reference { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        
        public bool IsPosted { get; set; }
        
        public ICollection<JournalEntryLine> Lines { get; set; } = new List<JournalEntryLine>();
    }
}
