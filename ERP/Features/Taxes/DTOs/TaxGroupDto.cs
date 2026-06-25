using System;
using System.Collections.Generic;

namespace ERP.Features.Taxes.DTOs
{
    public class TaxGroupDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? InputVatAccountId { get; set; }
        public Guid? OutputVatAccountId { get; set; }
        public bool IsActive { get; set; }
        
        public List<TaxRateDto> TaxRates { get; set; } = new();
    }

    public class TaxRateDto
    {
        public Guid Id { get; set; }
        public decimal RatePercentage { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
    }
}
