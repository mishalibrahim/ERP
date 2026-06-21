using System;
using System.Collections.Generic;

namespace ERP.DTOs.CompanySetup
{
    public class CompanyDetailsDto : UpdateCompanyGeneralDto
    {
        public string Id { get; set; } = string.Empty;
        
        public FinancialSetupDto? Financials { get; set; }
        public LocalizationSetupDto? Localization { get; set; }
        public AddressDetailsDto? RegisteredAddress { get; set; }
        public AddressDetailsDto? BillingAddress { get; set; }
        public VatSetupDto? VatDetails { get; set; }
        public CorporateTaxSetupDto? CorporateTax { get; set; }
        public TaxSetupDto? TaxConfiguration { get; set; }
        public SystemControlsDto? Controls { get; set; }
        public List<BankAccountDto>? BankAccounts { get; set; }
        public List<TaxGroupDto>? TaxGroups { get; set; }
        public List<DocumentNumberSeriesDto>? DocumentNumberSeries { get; set; }
        public List<PostingGroupDto>? PostingGroups { get; set; }
        public List<UserTenantAccessDto>? UserTenantAccesses { get; set; }
    }
}
