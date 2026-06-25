using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class CorporateTaxSetupDto
        {
            public bool? CtRegistered { get; set; }
            public string? CorporateTaxTrn { get; set; }
            public DateTime? FirstTaxPeriodStart { get; set; }
            public bool? FreeZonePerson { get; set; }
            public bool? QfzpStatus { get; set; }
            public bool? SmallBusinessRelief { get; set; }
        }
}

