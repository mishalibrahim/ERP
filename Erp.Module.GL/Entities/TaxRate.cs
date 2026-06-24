using System;
using Erp.Shared.Entities;

namespace Erp.Module.GL.Entities
{
    public class TaxRate : BaseEntity
    {
        public Guid TaxGroupId { get; set; }
        public TaxGroup? TaxGroup { get; set; }

        public decimal RatePercentage { get; set; }
        
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
