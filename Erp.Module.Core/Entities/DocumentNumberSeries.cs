using System;

namespace Erp.Module.Core.Entities
{
    public class DocumentNumberSeries
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string TenantId { get; set; } = string.Empty;
        public Tenant? Tenant { get; set; }

        public string DocumentType { get; set; } = string.Empty; // e.g., "Invoice", "Journal"
        
        public string Prefix { get; set; } = string.Empty;
        public long CurrentNumber { get; set; } = 0;
        public string? Suffix { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
