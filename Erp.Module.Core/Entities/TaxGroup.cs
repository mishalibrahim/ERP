using System;
using System.Collections.Generic;

namespace Erp.Module.Core.Entities
{
    public class TaxGroup
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        // Multi-tenancy link
        public string TenantId { get; set; } = string.Empty;
        public Tenant? Tenant { get; set; }

        public string Name { get; set; } = string.Empty; // e.g., "Standard", "Exempt", "Zero-Rated"
        public string? Description { get; set; }
        
        public bool IsActive { get; set; } = true;

        public ICollection<TaxRate> TaxRates { get; set; } = new List<TaxRate>();
    }
}
