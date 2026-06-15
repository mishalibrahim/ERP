using Erp.Module.Core.Data;
using Erp.Module.Core.Entities;
using ERP.DTOs.CompanySetup;
using ERP.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Services
{
    public class CompanySetupService : ICompanySetupService
    {
        private readonly CoreDbContext _context;

        public CompanySetupService(CoreDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyListItemDto>> GetAllAsync()
        {
            return await _context.Tenants
                .Where(t => t.IsActive)
                .Select(t => new CompanyListItemDto
                {
                    Id = t.Id,
                    CompanyName = t.CompanyName,
                    TradeName = t.TradeName,
                    CompanyCode = t.CompanyCode,
                    LicenseNumber = t.LicenseNumber,
                    LicenseType = t.LicenseType,
                    Country = t.Country,
                    Emirate = t.Emirate,
                    Status = t.Status,
                    RegistrationDate = t.RegistrationDate,
                    CreatedAt = t.CreatedAt,
                    UpdatedAt = t.UpdatedAt
                }).ToListAsync();
        }

        public async Task<CompanyListItemDto?> GetByIdAsync(string id)
        {
            var tenant = await _context.Tenants
                .Where(t => t.IsActive && t.Id == id)
                .FirstOrDefaultAsync();

            if (tenant == null) return null;

            return new CompanyListItemDto
            {
                Id = tenant.Id,
                CompanyName = tenant.CompanyName,
                TradeName = tenant.TradeName,
                CompanyCode = tenant.CompanyCode,
                LicenseNumber = tenant.LicenseNumber,
                LicenseType = tenant.LicenseType,
                Country = tenant.Country,
                Emirate = tenant.Emirate,
                Status = tenant.Status,
                RegistrationDate = tenant.RegistrationDate,
                CreatedAt = tenant.CreatedAt,
                UpdatedAt = tenant.UpdatedAt
            };
        }

        public async Task<string> CreateDraftAsync(CreateCompanyDto dto)
        {
            var tenant = new Tenant
            {
                CompanyName = dto.CompanyName,
                TradeName = dto.TradeName,
                CompanyCode = dto.CompanyCode,
                LicenseNumber = dto.LicenseNumber,
                LicenseType = dto.LicenseType,
                RegistrationDate = dto.RegistrationDate,
                LicenseExpiryDate = dto.LicenseExpiryDate,
                Country = dto.Country,
                Emirate = dto.Emirate,
                IsFreeZoneEntity = dto.IsFreeZoneEntity,
                IsDesignatedZone = dto.IsDesignatedZone,
                Status = "Draft",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            return tenant.Id;
        }

        public async Task<bool> UpdateCompanyAsync(string id, UpdateCompanyDto dto)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
            
            if (tenant == null)
            {
                return false;
            }

            // Step 1: General Info Partial Updates
            if (dto.Status != null) tenant.Status = dto.Status;
            if (dto.CompanyName != null) tenant.CompanyName = dto.CompanyName;
            if (dto.TradeName != null) tenant.TradeName = dto.TradeName;
            if (dto.CompanyCode != null) tenant.CompanyCode = dto.CompanyCode;
            if (dto.LicenseNumber != null) tenant.LicenseNumber = dto.LicenseNumber;
            if (dto.LicenseType != null) tenant.LicenseType = dto.LicenseType;
            if (dto.RegistrationDate.HasValue) tenant.RegistrationDate = dto.RegistrationDate.Value;
            if (dto.LicenseExpiryDate != null) tenant.LicenseExpiryDate = dto.LicenseExpiryDate;
            if (dto.Country != null) tenant.Country = dto.Country;
            if (dto.Emirate != null) tenant.Emirate = dto.Emirate;
            if (dto.IsFreeZoneEntity.HasValue) tenant.IsFreeZoneEntity = dto.IsFreeZoneEntity.Value;
            if (dto.IsDesignatedZone.HasValue) tenant.IsDesignatedZone = dto.IsDesignatedZone.Value;

            // Step 2: Financials Partial Update
            if (dto.Financials != null)
            {
                if (dto.Financials.FinancialYearStart.HasValue) tenant.Financials.FinancialYearStart = dto.Financials.FinancialYearStart;
                if (dto.Financials.BooksStartDate.HasValue) tenant.Financials.BooksStartDate = dto.Financials.BooksStartDate;
                if (dto.Financials.AccountingMethod != null) tenant.Financials.AccountingMethod = dto.Financials.AccountingMethod;
                if (dto.Financials.FiscalYear != null) tenant.Financials.FiscalYear = dto.Financials.FiscalYear;
                if (dto.Financials.BaseCurrency != null) tenant.Financials.BaseCurrency = dto.Financials.BaseCurrency;
                if (dto.Financials.ReportingCurrency != null) tenant.Financials.ReportingCurrency = dto.Financials.ReportingCurrency;
            }

            // Step 3: Localization Partial Update
            if (dto.Localization != null)
            {
                if (dto.Localization.OrganizationLanguage != null) tenant.Localization.OrganizationLanguage = dto.Localization.OrganizationLanguage;
                if (dto.Localization.InvoiceLanguage != null) tenant.Localization.InvoiceLanguage = dto.Localization.InvoiceLanguage;
                if (dto.Localization.TimeZone != null) tenant.Localization.TimeZone = dto.Localization.TimeZone;
                if (dto.Localization.DateFormat != null) tenant.Localization.DateFormat = dto.Localization.DateFormat;
            }

            // Step 4a: Registered Address
            if (dto.RegisteredAddress != null)
            {
                if (dto.RegisteredAddress.AddressLine1 != null) tenant.RegisteredAddress.AddressLine1 = dto.RegisteredAddress.AddressLine1;
                if (dto.RegisteredAddress.AddressLine2 != null) tenant.RegisteredAddress.AddressLine2 = dto.RegisteredAddress.AddressLine2;
                if (dto.RegisteredAddress.City != null) tenant.RegisteredAddress.City = dto.RegisteredAddress.City;
                if (dto.RegisteredAddress.Emirate != null) tenant.RegisteredAddress.Emirate = dto.RegisteredAddress.Emirate;
                if (dto.RegisteredAddress.POBox != null) tenant.RegisteredAddress.POBox = dto.RegisteredAddress.POBox;
                if (dto.RegisteredAddress.Country != null) tenant.RegisteredAddress.Country = dto.RegisteredAddress.Country;
                if (dto.RegisteredAddress.PhoneNumber != null) tenant.RegisteredAddress.PhoneNumber = dto.RegisteredAddress.PhoneNumber;
                if (dto.RegisteredAddress.FaxNumber != null) tenant.RegisteredAddress.FaxNumber = dto.RegisteredAddress.FaxNumber;
            }

            // Step 4b: Billing Address
            if (dto.BillingAddress != null)
            {
                if (dto.BillingAddress.AddressLine1 != null) tenant.BillingAddress.AddressLine1 = dto.BillingAddress.AddressLine1;
                if (dto.BillingAddress.AddressLine2 != null) tenant.BillingAddress.AddressLine2 = dto.BillingAddress.AddressLine2;
                if (dto.BillingAddress.City != null) tenant.BillingAddress.City = dto.BillingAddress.City;
                if (dto.BillingAddress.Emirate != null) tenant.BillingAddress.Emirate = dto.BillingAddress.Emirate;
                if (dto.BillingAddress.POBox != null) tenant.BillingAddress.POBox = dto.BillingAddress.POBox;
                if (dto.BillingAddress.Country != null) tenant.BillingAddress.Country = dto.BillingAddress.Country;
                if (dto.BillingAddress.PhoneNumber != null) tenant.BillingAddress.PhoneNumber = dto.BillingAddress.PhoneNumber;
                if (dto.BillingAddress.FaxNumber != null) tenant.BillingAddress.FaxNumber = dto.BillingAddress.FaxNumber;
            }

            // Step 5: VAT Details
            if (dto.VatDetails != null)
            {
                if (dto.VatDetails.VatRegistered.HasValue) tenant.VatDetails.VatRegistered = dto.VatDetails.VatRegistered.Value;
                if (dto.VatDetails.TrnLabel != null) tenant.VatDetails.TrnLabel = dto.VatDetails.TrnLabel;
                if (dto.VatDetails.TrnNumber != null) tenant.VatDetails.TrnNumber = dto.VatDetails.TrnNumber;
                if (dto.VatDetails.VatScheme != null) tenant.VatDetails.VatScheme = dto.VatDetails.VatScheme;
                if (dto.VatDetails.FilingFrequency != null) tenant.VatDetails.FilingFrequency = dto.VatDetails.FilingFrequency;
                if (dto.VatDetails.VatRegistrationDate.HasValue) tenant.VatDetails.VatRegistrationDate = dto.VatDetails.VatRegistrationDate;
            }

            // Step 6: Corporate Tax
            if (dto.CorporateTax != null)
            {
                if (dto.CorporateTax.CtRegistered.HasValue) tenant.CorporateTax.CtRegistered = dto.CorporateTax.CtRegistered.Value;
                if (dto.CorporateTax.CorporateTaxTrn != null) tenant.CorporateTax.CorporateTaxTrn = dto.CorporateTax.CorporateTaxTrn;
                if (dto.CorporateTax.FirstTaxPeriodStart.HasValue) tenant.CorporateTax.FirstTaxPeriodStart = dto.CorporateTax.FirstTaxPeriodStart;
                if (dto.CorporateTax.FreeZonePerson.HasValue) tenant.CorporateTax.FreeZonePerson = dto.CorporateTax.FreeZonePerson.Value;
                if (dto.CorporateTax.QfzpStatus.HasValue) tenant.CorporateTax.QfzpStatus = dto.CorporateTax.QfzpStatus.Value;
                if (dto.CorporateTax.SmallBusinessRelief.HasValue) tenant.CorporateTax.SmallBusinessRelief = dto.CorporateTax.SmallBusinessRelief.Value;
            }

            // Step 8: Controls
            if (dto.Controls != null)
            {
                if (dto.Controls.MultiCompanyEnable.HasValue) tenant.Controls.MultiCompanyEnable = dto.Controls.MultiCompanyEnable.Value;
                if (dto.Controls.AuditTrailEnable.HasValue) tenant.Controls.AuditTrailEnable = dto.Controls.AuditTrailEnable.Value;
                if (dto.Controls.ApprovalWorkflow.HasValue) tenant.Controls.ApprovalWorkflow = dto.Controls.ApprovalWorkflow.Value;
            }

            tenant.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var tenant = await _context.Tenants.FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
            
            if (tenant == null)
            {
                return false;
            }

            tenant.IsActive = false;
            tenant.UpdatedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
