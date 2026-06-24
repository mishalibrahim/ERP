using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.Taxes.DTOs
{
    public class UpdateTaxGroupDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? InputVatAccountId { get; set; }
        public Guid? OutputVatAccountId { get; set; }
        public bool IsActive { get; set; }

        public List<UpdateTaxRateDto> TaxRates { get; set; } = new();
    }

    public class UpdateTaxRateDto
    {
        public Guid? Id { get; set; } // If null, it's a new rate
        [Required]
        public decimal RatePercentage { get; set; }
        [Required]
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; }
    }
}
