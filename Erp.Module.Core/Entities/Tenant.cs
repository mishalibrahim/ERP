using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace Erp.Module.Core.Entities
{
    public class Tenant
    {
            // STEP 1: General Information
            public string Id { get; set; } = Guid.NewGuid().ToString();
            public string CompanyName { get; set; } = string.Empty;
            public string? TradeName { get; set; }
            public string CompanyCode { get; set; } = string.Empty;
            public string LicenseNumber { get; set; } = string.Empty;
            public string LicenseType { get; set; } = string.Empty;
            public DateTime RegistrationDate { get; set; }
            public DateTime? LicenseExpiryDate { get; set; }
            public string Country { get; set; } = "UAE";
            public string Emirate { get; set; } = string.Empty;
            public string? PlaceOfIncorporation { get; set; }
            public bool IsFreeZoneEntity { get; set; }
            public bool IsDesignatedZone { get; set; }

            public string Status { get; set; } = "Draft";
            
            public bool IsActive { get; set; } = true;
            public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
            public DateTime? UpdatedAt { get; set; }

            [System.ComponentModel.DataAnnotations.Timestamp]
            public byte[] RowVersion { get; set; }

            // FLATTENED 1-TO-1 SECTIONS (Owned Types)
            public FinancialSetup Financials { get; set; } = new();         // Step 2
            public LocalizationSetup Localization { get; set; } = new();    // Step 3
            public AddressDetails RegisteredAddress { get; set; } = new();  // Step 4a
            public AddressDetails BillingAddress { get; set; } = new();     // Step 4b
            public VatSetup VatDetails { get; set; } = new();               // Step 5
            public CorporateTaxSetup CorporateTax { get; set; } = new();    // Step 6
            public TaxSetup TaxConfiguration { get; set; } = new();         // Step 7
            public SystemControls Controls { get; set; } = new();           // Step 8
            public DocumentManagement Documents { get; set; } = new();      // Step 11

            // RELATIONAL 1-TO-MANY SECTIONS (Separate Tables)
            public List<BankAccount> BankAccounts { get; set; } = new();    // Step 9
            public ICollection<TaxGroup> TaxGroups { get; set; } = new List<TaxGroup>();
            public ICollection<DocumentNumberSeries> DocumentNumberSeries { get; set; } = new List<DocumentNumberSeries>();
            public ICollection<PostingGroup> PostingGroups { get; set; } = new List<PostingGroup>();
            public ICollection<UserTenantAccess> UserTenantAccesses { get; set; } = new List<UserTenantAccess>();
            
            // Note: Step 10 (Users) is handled by UserTenantAccess mapping
        }

        // --- THE 1-TO-1 OWNED TYPES (Flattened into the Tenants table) ---

        [Owned]
        public class FinancialSetup
        {
            public DateTime? FinancialYearStart { get; set; }
            public DateTime? FinancialYearEnd { get; set; }
            public DateTime? BooksStartDate { get; set; }
            public string AccountingMethod { get; set; } = "Accrual";
            public string FiscalYear { get; set; } = "Jan-Dec";
            public string BaseCurrency { get; set; } = "AED";
            public string? ReportingCurrency { get; set; }
        }

        [Owned]
        public class LocalizationSetup
        {
            public string OrganizationLanguage { get; set; } = "English";
            public List<string> CommunicationLanguages { get; set; } = new();
            public string InvoiceLanguage { get; set; } = "English";
            public string TimeZone { get; set; } = "Asia/Dubai";
            public string DateFormat { get; set; } = "DD/MM/YYYY";
        }

        [Owned]
        public class AddressDetails
        {
            public string AddressLine1 { get; set; } = string.Empty;
            public string? AddressLine2 { get; set; }
            public string City { get; set; } = string.Empty;
            public string Emirate { get; set; } = string.Empty;
            public string POBox { get; set; } = string.Empty;
            public string Country { get; set; } = "UAE";
            public string PhoneNumber { get; set; } = string.Empty;
            public string? FaxNumber { get; set; }
        }

        [Owned]
        public class VatSetup
        {
            public bool VatRegistered { get; set; }
            public string TrnLabel { get; set; } = "TRN";
            public string? TrnNumber { get; set; }
            public string VatScheme { get; set; } = "Standard";
            public string FilingFrequency { get; set; } = "Quarterly";
            public DateTime? VatRegistrationDate { get; set; }
            public DateTime? FirstVatPeriod { get; set; }
            public DateTime? VatReturnStartPeriod { get; set; }
            public DateTime? VatDeregistrationDate { get; set; }
        }

        [Owned]
        public class CorporateTaxSetup
        {
            public bool CtRegistered { get; set; }
            public string? CorporateTaxTrn { get; set; }
            public DateTime? FirstTaxPeriodStart { get; set; }
            public bool FreeZonePerson { get; set; }
            public bool QfzpStatus { get; set; }
            public bool SmallBusinessRelief { get; set; }
        }

        [Owned]
        public class SystemControls
        {
            public bool MultiCompanyEnable { get; set; } = false;
            public bool AuditTrailEnable { get; set; } = true;
            public bool ApprovalWorkflow { get; set; } = false;
            public string? DefaultCostCenterId { get; set; }
            public string? DefaultProjectId { get; set; }
        }

        [Owned]
        public class TaxSetup
        {
            public Guid? DefaultVatRateId { get; set; }
            public string? InputVatAccountId { get; set; }
            public string? OutputVatAccountId { get; set; }
        }

        [Owned]
        public class DocumentManagement
        {
            // For documents, we store the URL/Path of where the file is saved (e.g., AWS S3 or Azure Blob)
            public string? TradeLicenseUrl { get; set; }
            public string? MoaUrl { get; set; }
            public string? VatCertificateUrl { get; set; }
            public string? EmiratesIdUrl { get; set; }
            public string? PassportCopyUrl { get; set; }
        }
}
