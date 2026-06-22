using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class AddressDetailsDto
        {
            public string? AddressLine1 { get; set; }
            public string? AddressLine2 { get; set; }
            public string? City { get; set; }
            public string? Emirate { get; set; }
            public string? POBox { get; set; }
            public string? Country { get; set; }
            public string? PhoneNumber { get; set; }
            public string? FaxNumber { get; set; }
        }
}

