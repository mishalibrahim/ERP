using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class VatSetupDto
        {
            public bool? VatRegistered { get; set; }
            public string? TrnLabel { get; set; }
            public string? TrnNumber { get; set; }
            public string? VatScheme { get; set; }
            public string? FilingFrequency { get; set; }
            public DateTime? VatRegistrationDate { get; set; }
            public DateTime? FirstVatPeriod { get; set; }
            public DateTime? VatReturnStartPeriod { get; set; }
            public DateTime? VatDeregistrationDate { get; set; }
        }
}

