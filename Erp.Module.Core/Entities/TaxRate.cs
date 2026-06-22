using System;
using Erp.Shared.Entities;

namespace Erp.Module.Core.Entities
{
    public class TaxRate
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid TaxGroupId { get; set; }
        public TaxGroup? TaxGroup { get; set; }

        public decimal RatePercentage { get; set; }
        
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
