using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class UpdateCompanyGeneralDto : UpdateCompanyBaseDto
        {
            public string? Status { get; set; }
            public string? CompanyName { get; set; }
            public string? TradeName { get; set; }
            public string? CompanyCode { get; set; }
            public string? LicenseNumber { get; set; }
            public string? LicenseType { get; set; }
            public DateTime? RegistrationDate { get; set; }
            public DateTime? LicenseExpiryDate { get; set; }
            public string? Country { get; set; }
            public string? Emirate { get; set; }
            public string? PlaceOfIncorporation { get; set; }
            public bool? IsFreeZoneEntity { get; set; }
            public bool? IsDesignatedZone { get; set; }
        }
}

