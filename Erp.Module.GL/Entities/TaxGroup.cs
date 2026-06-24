using System;
using System.Collections.Generic;
using Erp.Shared.Entities;

namespace Erp.Module.GL.Entities
{
    public class TaxGroup : BaseEntity
    {
        public Guid TenantId { get; set; }

        public string Name { get; set; } = string.Empty; // e.g., "Standard", "Exempt", "Zero-Rated"
        public string? Description { get; set; }
        
        public Guid? InputVatAccountId { get; set; }
        public GlAccount? InputVatAccount { get; set; }
        
        public Guid? OutputVatAccountId { get; set; }
        public GlAccount? OutputVatAccount { get; set; }

        public ICollection<TaxRate> TaxRates { get; set; } = new List<TaxRate>();
    }
}
