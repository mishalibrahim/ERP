using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class LocalizationSetupDto
        {
            public string? OrganizationLanguage { get; set; }
            public List<string>? CommunicationLanguages { get; set; }
            public string? InvoiceLanguage { get; set; }
            public string? TimeZone { get; set; }
            public string? DateFormat { get; set; }
        }
}

