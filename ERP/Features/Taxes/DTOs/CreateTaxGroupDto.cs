using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.Taxes.DTOs
{
    public class CreateTaxGroupDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public Guid? InputVatAccountId { get; set; }
        public Guid? OutputVatAccountId { get; set; }

        public List<CreateTaxRateDto> TaxRates { get; set; } = new();
    }

    public class CreateTaxRateDto
    {
        [Required]
        public decimal RatePercentage { get; set; }
        [Required]
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
    }
}
