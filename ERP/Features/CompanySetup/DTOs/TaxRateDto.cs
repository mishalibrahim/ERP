using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class TaxRateDto
        {
            public Guid? Id { get; set; }
            public decimal RatePercentage { get; set; }
            public DateTime EffectiveFrom { get; set; }
            public DateTime? EffectiveTo { get; set; }
            public bool IsActive { get; set; } = true;
        }
}

