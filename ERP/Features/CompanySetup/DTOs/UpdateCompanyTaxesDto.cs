using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class UpdateCompanyTaxesDto : UpdateCompanyBaseDto
        {
            // Vat Details
            public bool? VatRegistered { get; set; }
            public string? TrnLabel { get; set; }
            public string? TrnNumber { get; set; }
            public string? VatScheme { get; set; }
            public string? FilingFrequency { get; set; }
            public DateTime? VatRegistrationDate { get; set; }
            public DateTime? FirstVatPeriod { get; set; }
            public DateTime? VatReturnStartPeriod { get; set; }
            public DateTime? VatDeregistrationDate { get; set; }
    
            // Corporate Tax
            public bool? CtRegistered { get; set; }
            public string? CorporateTaxTrn { get; set; }
            public DateTime? FirstTaxPeriodStart { get; set; }
            public bool? FreeZonePerson { get; set; }
            public bool? QfzpStatus { get; set; }
            public bool? SmallBusinessRelief { get; set; }
    
            // Tax Configuration
            public Guid? DefaultVatRateId { get; set; }
            public string? InputVatAccountId { get; set; }
            public string? OutputVatAccountId { get; set; }
    
            public List<TaxGroupDto>? TaxGroups { get; set; }
        }
}

