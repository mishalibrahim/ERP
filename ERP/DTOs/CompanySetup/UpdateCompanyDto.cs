using System;

namespace ERP.DTOs.CompanySetup
{
    public class UpdateCompanyDto
    {
        public string? Status { get; set; }

        // Step 1 Properties (optional for partial updates)
        public string? CompanyName { get; set; }
        public string? TradeName { get; set; }
        public string? CompanyCode { get; set; }
        public string? LicenseNumber { get; set; }
        public string? LicenseType { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? LicenseExpiryDate { get; set; }
        public string? Country { get; set; }
        public string? Emirate { get; set; }
        public bool? IsFreeZoneEntity { get; set; }
        public bool? IsDesignatedZone { get; set; }

        // Other Sections
        public FinancialSetupDto? Financials { get; set; }
        public LocalizationSetupDto? Localization { get; set; }
        public AddressDetailsDto? RegisteredAddress { get; set; }
        public AddressDetailsDto? BillingAddress { get; set; }
        public VatSetupDto? VatDetails { get; set; }
        public CorporateTaxSetupDto? CorporateTax { get; set; }
        public SystemControlsDto? Controls { get; set; }
    }

    public class FinancialSetupDto
    {
        public DateTime? FinancialYearStart { get; set; }
        public DateTime? BooksStartDate { get; set; }
        public string? AccountingMethod { get; set; }
        public string? FiscalYear { get; set; }
        public string? BaseCurrency { get; set; }
        public string? ReportingCurrency { get; set; }
    }

    public class LocalizationSetupDto
    {
        public string? OrganizationLanguage { get; set; }
        public string? InvoiceLanguage { get; set; }
        public string? TimeZone { get; set; }
        public string? DateFormat { get; set; }
    }

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

    public class VatSetupDto
    {
        public bool? VatRegistered { get; set; }
        public string? TrnLabel { get; set; }
        public string? TrnNumber { get; set; }
        public string? VatScheme { get; set; }
        public string? FilingFrequency { get; set; }
        public DateTime? VatRegistrationDate { get; set; }
    }

    public class CorporateTaxSetupDto
    {
        public bool? CtRegistered { get; set; }
        public string? CorporateTaxTrn { get; set; }
        public DateTime? FirstTaxPeriodStart { get; set; }
        public bool? FreeZonePerson { get; set; }
        public bool? QfzpStatus { get; set; }
        public bool? SmallBusinessRelief { get; set; }
    }

    public class SystemControlsDto
    {
        public bool? MultiCompanyEnable { get; set; }
        public bool? AuditTrailEnable { get; set; }
        public bool? ApprovalWorkflow { get; set; }
    }
}
