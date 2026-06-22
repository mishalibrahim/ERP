using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class TaxSetupDto
        {
            public Guid? DefaultVatRateId { get; set; }
            public string? InputVatAccountId { get; set; }
            public string? OutputVatAccountId { get; set; }
        }
}

