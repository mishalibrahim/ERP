using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class TaxGroupDto
        {
            public Guid? Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string? Description { get; set; }
            public bool IsActive { get; set; } = true;
            public List<TaxRateDto> TaxRates { get; set; } = new List<TaxRateDto>();
        }
}

