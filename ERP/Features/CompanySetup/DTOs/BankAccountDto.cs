using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.Features.CompanySetup.DTOs
{
    public class BankAccountDto
        {
            public Guid? Id { get; set; }
            public bool IsPrimary { get; set; }
            public string BankName { get; set; } = string.Empty;
            public string AccountName { get; set; } = string.Empty;
            public string AccountNumber { get; set; } = string.Empty;
            public string? Iban { get; set; }
            public string? SwiftCode { get; set; }
            public string Currency { get; set; } = "AED";
        }
}

