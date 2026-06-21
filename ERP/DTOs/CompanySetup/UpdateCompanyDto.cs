using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ERP.DTOs.CompanySetup
{
    public abstract class UpdateCompanyBaseDto
    {
        [Required]
        [MinLength(8, ErrorMessage = "RowVersion is required and must be valid.")]
        public byte[] RowVersion { get; set; } = Array.Empty<byte>();
    }

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

    public class UpdateCompanyFinancialsDto : UpdateCompanyBaseDto
    {
        public DateTime? FinancialYearStart { get; set; }
        public DateTime? FinancialYearEnd { get; set; }
        public DateTime? BooksStartDate { get; set; }
        public string? AccountingMethod { get; set; }
        public string? FiscalYear { get; set; }
        public string? BaseCurrency { get; set; }
        public string? ReportingCurrency { get; set; }
    }

    public class UpdateCompanyLocalizationDto : UpdateCompanyBaseDto
    {
        public string? OrganizationLanguage { get; set; }
        public List<string>? CommunicationLanguages { get; set; }
        public string? InvoiceLanguage { get; set; }
        public string? TimeZone { get; set; }
        public string? DateFormat { get; set; }
    }

    public class UpdateCompanyAddressesDto : UpdateCompanyBaseDto
    {
        public AddressDetailsDto? RegisteredAddress { get; set; }
        public AddressDetailsDto? BillingAddress { get; set; }
    }

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

    public class UpdateCompanySystemControlsDto : UpdateCompanyBaseDto
    {
        public bool? MultiCompanyEnable { get; set; }
        public bool? AuditTrailEnable { get; set; }
        public bool? ApprovalWorkflow { get; set; }
        public string? DefaultCostCenterId { get; set; }
        public string? DefaultProjectId { get; set; }

        public List<DocumentNumberSeriesDto>? DocumentNumberSeries { get; set; }
        public List<PostingGroupDto>? PostingGroups { get; set; }
    }

    public class UpdateCompanyBankAccountsDto : UpdateCompanyBaseDto
    {
        public List<BankAccountDto>? BankAccounts { get; set; }
    }

    public class UpdateCompanyUsersDto : UpdateCompanyBaseDto
    {
        public string? Status { get; set; }
        public List<UserTenantAccessDto>? UserTenantAccess { get; set; }
    }


    public class FinancialSetupDto
    {
        public DateTime? FinancialYearStart { get; set; }
        public DateTime? FinancialYearEnd { get; set; }
        public DateTime? BooksStartDate { get; set; }
        public string? AccountingMethod { get; set; }
        public string? FiscalYear { get; set; }
        public string? BaseCurrency { get; set; }
        public string? ReportingCurrency { get; set; }
    }

    public class LocalizationSetupDto
    {
        public string? OrganizationLanguage { get; set; }
        public List<string>? CommunicationLanguages { get; set; }
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
        public DateTime? FirstVatPeriod { get; set; }
        public DateTime? VatReturnStartPeriod { get; set; }
        public DateTime? VatDeregistrationDate { get; set; }
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
        public string? DefaultCostCenterId { get; set; }
        public string? DefaultProjectId { get; set; }
    }

    public class TaxSetupDto
    {
        public Guid? DefaultVatRateId { get; set; }
        public string? InputVatAccountId { get; set; }
        public string? OutputVatAccountId { get; set; }
    }

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

    public class TaxGroupDto
    {
        public Guid? Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsActive { get; set; } = true;
        public List<TaxRateDto> TaxRates { get; set; } = new List<TaxRateDto>();
    }

    public class TaxRateDto
    {
        public Guid? Id { get; set; }
        public decimal RatePercentage { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public DateTime? EffectiveTo { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class DocumentNumberSeriesDto
    {
        public Guid? Id { get; set; }
        public string DocumentType { get; set; } = string.Empty;
        public string Prefix { get; set; } = string.Empty;
        public long CurrentNumber { get; set; } = 0;
        public string? Suffix { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class PostingGroupDto
    {
        public Guid? Id { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public string? ReceivablesAccountId { get; set; }
        public string? PayablesAccountId { get; set; }
        public string? InventoryAccountId { get; set; }
        public string? CogsAccountId { get; set; }
        public bool IsActive { get; set; } = true;
    }

    public class UserTenantAccessDto
    {
        public Guid? Id { get; set; }
        public string? UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string? Password { get; set; } // Used for new users
        public string Role { get; set; } = "User";
        public bool IsActive { get; set; } = true;
    }
}
