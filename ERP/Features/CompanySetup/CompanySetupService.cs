using Erp.Module.Core.Data;
using Erp.Module.Core.Entities;
using Erp.Module.GL.Data;
using Erp.Module.GL.Entities;
using ERP.Features.CompanySetup.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ERP.Features.CompanySetup
{
    public class CompanySetupService : ICompanySetupService
    {
        private readonly CoreDbContext _context;
        private readonly GlDbContext _glContext;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly Erp.Shared.Interfaces.ICurrentUserService _currentUserService;

        public CompanySetupService(CoreDbContext context, GlDbContext glContext, Erp.Shared.Interfaces.ICurrentUserService currentUserService, IPasswordHasher<User> passwordHasher = null)
        {
            _context = context;
            _glContext = glContext;
            _currentUserService = currentUserService;
            _passwordHasher = passwordHasher ?? new PasswordHasher<User>();
        }

        public async Task<List<CompanyListItemDto>> GetAllAsync()
        {
            var query = _context.Tenants.AsQueryable();
            if (_currentUserService.IsSuperAdmin)
            {
                query = query.IgnoreQueryFilters();
            }

            return await query
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
                    UpdatedAt = t.ModifiedAt
                }).ToListAsync();
        }

        public async Task<CompanyDetailsDto?> GetByIdAsync(Guid id)
        {
            var query = _context.Tenants.AsQueryable();
            if (_currentUserService.IsSuperAdmin)
            {
                query = query.IgnoreQueryFilters();
            }

            var tenant = await query
                .Include(t => t.BankAccounts)
                .Include(t => t.DocumentNumberSeries)
                .Include(t => t.PostingGroups)
                .Include(t => t.UserTenantAccesses).ThenInclude(u => u.User)
                .Where(t => t.IsActive && t.Id == id)
                .FirstOrDefaultAsync();

            if (tenant == null) return null;

            var taxQuery = _glContext.TaxGroups.AsQueryable();
            if (_currentUserService.IsSuperAdmin)
            {
                taxQuery = taxQuery.IgnoreQueryFilters();
            }

            var taxGroups = await taxQuery
                .Include(tg => tg.TaxRates)
                .Where(tg => tg.TenantId == id && tg.IsActive)
                .ToListAsync();

            return new CompanyDetailsDto
            {
                Id = tenant.Id,
                RowVersion = tenant.RowVersion,
                Status = tenant.Status,
                CompanyName = tenant.CompanyName,
                TradeName = tenant.TradeName,
                CompanyCode = tenant.CompanyCode,
                LicenseNumber = tenant.LicenseNumber,
                LicenseType = tenant.LicenseType,
                RegistrationDate = tenant.RegistrationDate,
                LicenseExpiryDate = tenant.LicenseExpiryDate,
                Country = tenant.Country,
                Emirate = tenant.Emirate,
                PlaceOfIncorporation = tenant.PlaceOfIncorporation,
                IsFreeZoneEntity = tenant.IsFreeZoneEntity,
                IsDesignatedZone = tenant.IsDesignatedZone,

                Financials = new FinancialSetupDto
                {
                    FinancialYearStart = tenant.Financials.FinancialYearStart,
                    FinancialYearEnd = tenant.Financials.FinancialYearEnd,
                    BooksStartDate = tenant.Financials.BooksStartDate,
                    AccountingMethod = tenant.Financials.AccountingMethod,
                    FiscalYear = tenant.Financials.FiscalYear,
                    BaseCurrency = tenant.Financials.BaseCurrency,
                    ReportingCurrency = tenant.Financials.ReportingCurrency
                },
                Localization = new LocalizationSetupDto
                {
                    OrganizationLanguage = tenant.Localization.OrganizationLanguage,
                    CommunicationLanguages = tenant.Localization.CommunicationLanguages,
                    InvoiceLanguage = tenant.Localization.InvoiceLanguage,
                    TimeZone = tenant.Localization.TimeZone,
                    DateFormat = tenant.Localization.DateFormat
                },
                RegisteredAddress = new AddressDetailsDto
                {
                    AddressLine1 = tenant.RegisteredAddress.AddressLine1,
                    AddressLine2 = tenant.RegisteredAddress.AddressLine2,
                    City = tenant.RegisteredAddress.City,
                    Emirate = tenant.RegisteredAddress.Emirate,
                    POBox = tenant.RegisteredAddress.POBox,
                    Country = tenant.RegisteredAddress.Country,
                    PhoneNumber = tenant.RegisteredAddress.PhoneNumber,
                    FaxNumber = tenant.RegisteredAddress.FaxNumber
                },
                BillingAddress = new AddressDetailsDto
                {
                    AddressLine1 = tenant.BillingAddress.AddressLine1,
                    AddressLine2 = tenant.BillingAddress.AddressLine2,
                    City = tenant.BillingAddress.City,
                    Emirate = tenant.BillingAddress.Emirate,
                    POBox = tenant.BillingAddress.POBox,
                    Country = tenant.BillingAddress.Country,
                    PhoneNumber = tenant.BillingAddress.PhoneNumber,
                    FaxNumber = tenant.BillingAddress.FaxNumber
                },
                VatDetails = new VatSetupDto
                {
                    VatRegistered = tenant.VatDetails.VatRegistered,
                    TrnLabel = tenant.VatDetails.TrnLabel,
                    TrnNumber = tenant.VatDetails.TrnNumber,
                    VatScheme = tenant.VatDetails.VatScheme,
                    FilingFrequency = tenant.VatDetails.FilingFrequency,
                    VatRegistrationDate = tenant.VatDetails.VatRegistrationDate,
                    FirstVatPeriod = tenant.VatDetails.FirstVatPeriod,
                    VatReturnStartPeriod = tenant.VatDetails.VatReturnStartPeriod,
                    VatDeregistrationDate = tenant.VatDetails.VatDeregistrationDate
                },
                CorporateTax = new CorporateTaxSetupDto
                {
                    CtRegistered = tenant.CorporateTax.CtRegistered,
                    CorporateTaxTrn = tenant.CorporateTax.CorporateTaxTrn,
                    FirstTaxPeriodStart = tenant.CorporateTax.FirstTaxPeriodStart,
                    FreeZonePerson = tenant.CorporateTax.FreeZonePerson,
                    QfzpStatus = tenant.CorporateTax.QfzpStatus,
                    SmallBusinessRelief = tenant.CorporateTax.SmallBusinessRelief
                },
                TaxConfiguration = new TaxSetupDto
                {
                    DefaultVatRateId = tenant.TaxConfiguration.DefaultVatRateId,
                    InputVatAccountId = tenant.TaxConfiguration.InputVatAccountId,
                    OutputVatAccountId = tenant.TaxConfiguration.OutputVatAccountId
                },
                Controls = new SystemControlsDto
                {
                    MultiCompanyEnable = tenant.Controls.MultiCompanyEnable,
                    AuditTrailEnable = tenant.Controls.AuditTrailEnable,
                    ApprovalWorkflow = tenant.Controls.ApprovalWorkflow,
                    DefaultCostCenterId = tenant.Controls.DefaultCostCenterId,
                    DefaultProjectId = tenant.Controls.DefaultProjectId
                },
                BankAccounts = tenant.BankAccounts.Select(b => new BankAccountDto
                {
                    Id = b.Id,
                    IsPrimary = b.IsPrimary,
                    BankName = b.BankName,
                    AccountName = b.AccountName,
                    AccountNumber = b.AccountNumber,
                    Iban = b.Iban,
                    SwiftCode = b.SwiftCode,
                    Currency = b.Currency
                }).ToList(),
                TaxGroups = taxGroups.Select(tg => new TaxGroupDto
                {
                    Id = tg.Id,
                    Name = tg.Name,
                    Description = tg.Description,
                    IsActive = tg.IsActive,
                    TaxRates = tg.TaxRates.Select(tr => new TaxRateDto
                    {
                        Id = tr.Id,
                        RatePercentage = tr.RatePercentage,
                        EffectiveFrom = tr.EffectiveFrom,
                        EffectiveTo = tr.EffectiveTo,
                        IsActive = tr.IsActive
                    }).ToList()
                }).ToList(),
                DocumentNumberSeries = tenant.DocumentNumberSeries.Select(d => new DocumentNumberSeriesDto
                {
                    Id = d.Id,
                    DocumentType = d.DocumentType,
                    Prefix = d.Prefix,
                    CurrentNumber = d.CurrentNumber,
                    Suffix = d.Suffix,
                    IsActive = d.IsActive
                }).ToList(),
                PostingGroups = tenant.PostingGroups.Select(p => new PostingGroupDto
                {
                    Id = p.Id,
                    GroupName = p.GroupName,
                    Type = p.Type,
                    ReceivablesAccountId = p.ReceivablesAccountId,
                    PayablesAccountId = p.PayablesAccountId,
                    InventoryAccountId = p.InventoryAccountId,
                    CogsAccountId = p.CogsAccountId,
                    IsActive = p.IsActive
                }).ToList(),
                UserTenantAccesses = tenant.UserTenantAccesses.Select(u => new UserTenantAccessDto
                {
                    Id = u.Id,
                    UserId = u.UserId,
                    Email = u.User?.Email ?? string.Empty,
                    FirstName = u.User?.FirstName,
                    LastName = u.User?.LastName,
                    RoleId = u.RoleId,
                    RoleName = u.Role?.Name,
                    IsActive = u.IsActive
                }).ToList()
            };
        }

        public async Task<(Guid Id, byte[] RowVersion)> CreateDraftAsync(CreateCompanyDto dto)
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
                PlaceOfIncorporation = dto.PlaceOfIncorporation,
                IsFreeZoneEntity = dto.IsFreeZoneEntity,
                IsDesignatedZone = dto.IsDesignatedZone,
                Status = "Draft",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.Tenants.Add(tenant);
            await _context.SaveChangesAsync();

            return (tenant.Id, tenant.RowVersion);
        }

        private async Task<Tenant?> GetTenantForUpdateAsync(Guid id)
        {
            // Only loads the Tenant row + owned types (flattened columns).
            var query = _context.Tenants.AsQueryable();
            if (_currentUserService.IsSuperAdmin)
            {
                query = query.IgnoreQueryFilters();
            }

            return await query
                .FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
        }

        public async Task<byte[]?> UpdateGeneralInfoAsync(Guid id, UpdateCompanyGeneralDto dto)
        {
            var tenant = await GetTenantForUpdateAsync(id);
            if (tenant == null) return null;

            if (!tenant.RowVersion.SequenceEqual(dto.RowVersion))
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException();
            }

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
            if (dto.PlaceOfIncorporation != null) tenant.PlaceOfIncorporation = dto.PlaceOfIncorporation;
            if (dto.IsFreeZoneEntity.HasValue) tenant.IsFreeZoneEntity = dto.IsFreeZoneEntity.Value;
            if (dto.IsDesignatedZone.HasValue) tenant.IsDesignatedZone = dto.IsDesignatedZone.Value;

            tenant.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return tenant.RowVersion;
        }

        public async Task<byte[]?> UpdateFinancialsAsync(Guid id, UpdateCompanyFinancialsDto dto)
        {
            var tenant = await GetTenantForUpdateAsync(id);
            if (tenant == null) return null;

            if (!tenant.RowVersion.SequenceEqual(dto.RowVersion))
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException();
            }

            if (dto.FinancialYearStart.HasValue) tenant.Financials.FinancialYearStart = dto.FinancialYearStart;
            if (dto.FinancialYearEnd.HasValue) tenant.Financials.FinancialYearEnd = dto.FinancialYearEnd;
            if (dto.BooksStartDate.HasValue) tenant.Financials.BooksStartDate = dto.BooksStartDate;
            if (dto.AccountingMethod != null) tenant.Financials.AccountingMethod = dto.AccountingMethod;
            if (dto.FiscalYear != null) tenant.Financials.FiscalYear = dto.FiscalYear;
            if (dto.BaseCurrency != null) tenant.Financials.BaseCurrency = dto.BaseCurrency;
            if (dto.ReportingCurrency != null) tenant.Financials.ReportingCurrency = dto.ReportingCurrency;

            tenant.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return tenant.RowVersion;
        }

        public async Task<byte[]?> UpdateLocalizationAsync(Guid id, UpdateCompanyLocalizationDto dto)
        {
            var tenant = await GetTenantForUpdateAsync(id);
            if (tenant == null) return null;

            if (!tenant.RowVersion.SequenceEqual(dto.RowVersion))
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException();
            }

            if (dto.OrganizationLanguage != null) tenant.Localization.OrganizationLanguage = dto.OrganizationLanguage;
            if (dto.CommunicationLanguages != null) tenant.Localization.CommunicationLanguages = dto.CommunicationLanguages;
            if (dto.InvoiceLanguage != null) tenant.Localization.InvoiceLanguage = dto.InvoiceLanguage;
            if (dto.TimeZone != null) tenant.Localization.TimeZone = dto.TimeZone;
            if (dto.DateFormat != null) tenant.Localization.DateFormat = dto.DateFormat;

            tenant.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return tenant.RowVersion;
        }

        public async Task<byte[]?> UpdateAddressesAsync(Guid id, UpdateCompanyAddressesDto dto)
        {
            var tenant = await GetTenantForUpdateAsync(id);
            if (tenant == null) return null;

            if (!tenant.RowVersion.SequenceEqual(dto.RowVersion))
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException();
            }

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

            tenant.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return tenant.RowVersion;
        }

        public async Task<byte[]?> UpdateTaxesAsync(Guid id, UpdateCompanyTaxesDto dto)
        {
            var tenant = await GetTenantForUpdateAsync(id);
            if (tenant == null) return null;

            if (!tenant.RowVersion.SequenceEqual(dto.RowVersion))
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException();
            }

            // VAT Details (owned type — safe to update via tenant)
            if (dto.VatRegistered.HasValue) tenant.VatDetails.VatRegistered = dto.VatRegistered.Value;
            if (dto.TrnLabel != null) tenant.VatDetails.TrnLabel = dto.TrnLabel;
            if (dto.TrnNumber != null) tenant.VatDetails.TrnNumber = dto.TrnNumber;
            if (dto.VatScheme != null) tenant.VatDetails.VatScheme = dto.VatScheme;
            if (dto.FilingFrequency != null) tenant.VatDetails.FilingFrequency = dto.FilingFrequency;
            if (dto.VatRegistrationDate.HasValue) tenant.VatDetails.VatRegistrationDate = dto.VatRegistrationDate;
            if (dto.FirstVatPeriod.HasValue) tenant.VatDetails.FirstVatPeriod = dto.FirstVatPeriod;
            if (dto.VatReturnStartPeriod.HasValue) tenant.VatDetails.VatReturnStartPeriod = dto.VatReturnStartPeriod;
            if (dto.VatDeregistrationDate.HasValue) tenant.VatDetails.VatDeregistrationDate = dto.VatDeregistrationDate;

            // Corporate Tax (owned type — safe to update via tenant)
            if (dto.CtRegistered.HasValue) tenant.CorporateTax.CtRegistered = dto.CtRegistered.Value;
            if (dto.CorporateTaxTrn != null) tenant.CorporateTax.CorporateTaxTrn = dto.CorporateTaxTrn;
            if (dto.FirstTaxPeriodStart.HasValue) tenant.CorporateTax.FirstTaxPeriodStart = dto.FirstTaxPeriodStart;
            if (dto.FreeZonePerson.HasValue) tenant.CorporateTax.FreeZonePerson = dto.FreeZonePerson.Value;
            if (dto.QfzpStatus.HasValue) tenant.CorporateTax.QfzpStatus = dto.QfzpStatus.Value;
            if (dto.SmallBusinessRelief.HasValue) tenant.CorporateTax.SmallBusinessRelief = dto.SmallBusinessRelief.Value;

            // Tax Configuration (owned type — safe to update via tenant)
            if (dto.DefaultVatRateId.HasValue) tenant.TaxConfiguration.DefaultVatRateId = dto.DefaultVatRateId;
            if (dto.InputVatAccountId != null) tenant.TaxConfiguration.InputVatAccountId = dto.InputVatAccountId;
            if (dto.OutputVatAccountId != null) tenant.TaxConfiguration.OutputVatAccountId = dto.OutputVatAccountId;

            // TaxGroups + TaxRates (child collections — use direct DbSet access via _glContext)
            if (dto.TaxGroups != null)
            {
                var taxQuery = _glContext.TaxGroups.AsQueryable();
                if (_currentUserService.IsSuperAdmin) taxQuery = taxQuery.IgnoreQueryFilters();

                var existingGroups = await taxQuery
                    .Include(tg => tg.TaxRates)
                    .Where(tg => tg.TenantId == id)
                    .ToListAsync();

                var incomingTgIds = dto.TaxGroups.Where(tg => tg.Id.HasValue).Select(tg => tg.Id.Value).ToList();
                var removedGroups = existingGroups.Where(tg => !incomingTgIds.Contains(tg.Id)).ToList();
                _glContext.RemoveRange(removedGroups);

                foreach (var tgDto in dto.TaxGroups)
                {
                    if (tgDto.Id.HasValue)
                    {
                        var existingTg = existingGroups.FirstOrDefault(tg => tg.Id == tgDto.Id.Value);
                        if (existingTg != null)
                        {
                            existingTg.Name = tgDto.Name;
                            existingTg.Description = tgDto.Description;
                            existingTg.IsActive = tgDto.IsActive;

                            var incomingRateIds = tgDto.TaxRates.Where(tr => tr.Id.HasValue).Select(tr => tr.Id.Value).ToList();
                            var removedRates = existingTg.TaxRates.Where(tr => !incomingRateIds.Contains(tr.Id)).ToList();
                            _glContext.RemoveRange(removedRates);

                            foreach (var trDto in tgDto.TaxRates)
                            {
                                if (trDto.Id.HasValue)
                                {
                                    var existingTr = existingTg.TaxRates.FirstOrDefault(tr => tr.Id == trDto.Id.Value);
                                    if (existingTr != null)
                                    {
                                        existingTr.RatePercentage = trDto.RatePercentage;
                                        existingTr.EffectiveFrom = trDto.EffectiveFrom;
                                        existingTr.EffectiveTo = trDto.EffectiveTo;
                                        existingTr.IsActive = trDto.IsActive;
                                    }
                                }
                                else
                                {
                                    _glContext.TaxRates.Add(new TaxRate
                                    {
                                        TaxGroupId = existingTg.Id,
                                        RatePercentage = trDto.RatePercentage,
                                        EffectiveFrom = trDto.EffectiveFrom,
                                        EffectiveTo = trDto.EffectiveTo,
                                        IsActive = trDto.IsActive
                                    });
                                }
                            }
                        }
                    }
                    else
                    {
                        var newTg = new TaxGroup
                        {
                            TenantId = id,
                            Name = tgDto.Name,
                            Description = tgDto.Description,
                            IsActive = tgDto.IsActive
                        };
                        _glContext.TaxGroups.Add(newTg);

                        foreach (var trDto in tgDto.TaxRates)
                        {
                            _glContext.TaxRates.Add(new TaxRate
                            {
                                TaxGroupId = newTg.Id,
                                RatePercentage = trDto.RatePercentage,
                                EffectiveFrom = trDto.EffectiveFrom,
                                EffectiveTo = trDto.EffectiveTo,
                                IsActive = trDto.IsActive
                            });
                        }
                    }
                }
            }

            await _context.SaveChangesAsync();
            await _glContext.SaveChangesAsync();
            return tenant.RowVersion;
        }

        public async Task<byte[]?> UpdateSystemControlsAsync(Guid id, UpdateCompanySystemControlsDto dto)
        {
            var query = _context.Tenants.AsQueryable();
            if (_currentUserService.IsSuperAdmin) query = query.IgnoreQueryFilters();

            var tenantInfo = await query.AsNoTracking()
                .Where(t => t.Id == id && t.IsActive)
                .Select(t => new { t.RowVersion })
                .FirstOrDefaultAsync();

            if (tenantInfo == null) return null;

            if (!tenantInfo.RowVersion.SequenceEqual(dto.RowVersion))
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException("Another user has updated this record.");
            }

            // We need the tracked Tenant ONLY for owned types, NOT children
            var tenant = await query.FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
            if (tenant != null)
            {
                if (dto.MultiCompanyEnable.HasValue) tenant.Controls.MultiCompanyEnable = dto.MultiCompanyEnable.Value;
                if (dto.AuditTrailEnable.HasValue) tenant.Controls.AuditTrailEnable = dto.AuditTrailEnable.Value;
                if (dto.ApprovalWorkflow.HasValue) tenant.Controls.ApprovalWorkflow = dto.ApprovalWorkflow.Value;
                if (dto.DefaultCostCenterId != null) tenant.Controls.DefaultCostCenterId = dto.DefaultCostCenterId;
                if (dto.DefaultProjectId != null) tenant.Controls.DefaultProjectId = dto.DefaultProjectId;
            }

            // Document Number Series
            if (dto.DocumentNumberSeries != null)
            {
                var docQuery = _context.DocumentNumberSeries.AsQueryable();
                if (_currentUserService.IsSuperAdmin) docQuery = docQuery.IgnoreQueryFilters();

                var existingDocs = await docQuery
                    .Where(d => d.TenantId == id)
                    .ToListAsync();

                var incomingIds = dto.DocumentNumberSeries.Where(d => d.Id.HasValue).Select(d => d.Id.Value).ToList();
                var removedDocs = existingDocs.Where(d => !incomingIds.Contains(d.Id)).ToList();
                _context.RemoveRange(removedDocs);

                foreach (var dDto in dto.DocumentNumberSeries)
                {
                    if (dDto.Id.HasValue)
                    {
                        var existing = existingDocs.FirstOrDefault(d => d.Id == dDto.Id.Value);
                        if (existing != null)
                        {
                            existing.DocumentType = dDto.DocumentType;
                            existing.Prefix = dDto.Prefix;
                            existing.CurrentNumber = dDto.CurrentNumber;
                            existing.Suffix = dDto.Suffix;
                            existing.IsActive = dDto.IsActive;
                        }
                    }
                    else
                    {
                        _context.DocumentNumberSeries.Add(new DocumentNumberSeries
                        {
                            TenantId = id,
                            DocumentType = dDto.DocumentType,
                            Prefix = dDto.Prefix,
                            CurrentNumber = dDto.CurrentNumber,
                            Suffix = dDto.Suffix,
                            IsActive = dDto.IsActive
                        });
                    }
                }
            }

            // Posting Groups
            if (dto.PostingGroups != null)
            {
                var pgQuery = _context.PostingGroups.AsQueryable();
                if (_currentUserService.IsSuperAdmin) pgQuery = pgQuery.IgnoreQueryFilters();

                var existingPg = await pgQuery
                    .Where(p => p.TenantId == id)
                    .ToListAsync();

                var incomingIds = dto.PostingGroups.Where(p => p.Id.HasValue).Select(p => p.Id.Value).ToList();
                var removedPg = existingPg.Where(p => !incomingIds.Contains(p.Id)).ToList();
                _context.RemoveRange(removedPg);

                foreach (var pDto in dto.PostingGroups)
                {
                    if (pDto.Id.HasValue)
                    {
                        var existing = existingPg.FirstOrDefault(p => p.Id == pDto.Id.Value);
                        if (existing != null)
                        {
                            existing.GroupName = pDto.GroupName;
                            existing.Type = pDto.Type;
                            existing.ReceivablesAccountId = pDto.ReceivablesAccountId;
                            existing.PayablesAccountId = pDto.PayablesAccountId;
                            existing.InventoryAccountId = pDto.InventoryAccountId;
                            existing.CogsAccountId = pDto.CogsAccountId;
                            existing.IsActive = pDto.IsActive;
                        }
                    }
                    else
                    {
                        _context.PostingGroups.Add(new PostingGroup
                        {
                            TenantId = id,
                            GroupName = pDto.GroupName,
                            Type = pDto.Type,
                            ReceivablesAccountId = pDto.ReceivablesAccountId,
                            PayablesAccountId = pDto.PayablesAccountId,
                            InventoryAccountId = pDto.InventoryAccountId,
                            CogsAccountId = pDto.CogsAccountId,
                            IsActive = pDto.IsActive
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            var currentRv = await query.AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => t.RowVersion)
                .FirstAsync();
            return currentRv;
        }

        public async Task<byte[]?> UpdateBankAccountsAsync(Guid id, UpdateCompanyBankAccountsDto dto)
        {
            var query = _context.Tenants.AsQueryable();
            if (_currentUserService.IsSuperAdmin) query = query.IgnoreQueryFilters();

            // Concurrency check: read RowVersion without tracking the Tenant entity
            var tenantInfo = await query.AsNoTracking()
                .Where(t => t.Id == id && t.IsActive)
                .Select(t => new { t.RowVersion })
                .FirstOrDefaultAsync();

            if (tenantInfo == null) return null;

            if (!tenantInfo.RowVersion.SequenceEqual(dto.RowVersion))
            {
                throw new DbUpdateConcurrencyException("Another user has updated this record.");
            }

            if (dto.BankAccounts != null)
            {
                var bankQuery = _context.BankAccounts.AsQueryable();
                if (_currentUserService.IsSuperAdmin) bankQuery = bankQuery.IgnoreQueryFilters();

                // Load existing bank accounts directly (NOT through Tenant navigation)
                var existingBanks = await bankQuery
                    .Where(b => b.TenantId == id)
                    .ToListAsync();

                var incomingIds = dto.BankAccounts.Where(b => b.Id.HasValue).Select(b => b.Id.Value).ToList();
                var removedBanks = existingBanks.Where(b => !incomingIds.Contains(b.Id)).ToList();

                _context.RemoveRange(removedBanks);

                foreach (var bankDto in dto.BankAccounts)
                {
                    if (bankDto.Id.HasValue)
                    {
                        var existing = existingBanks.FirstOrDefault(b => b.Id == bankDto.Id.Value);
                        if (existing != null)
                        {
                            existing.IsPrimary = bankDto.IsPrimary;
                            existing.BankName = bankDto.BankName;
                            existing.AccountName = bankDto.AccountName;
                            existing.AccountNumber = bankDto.AccountNumber;
                            existing.Iban = bankDto.Iban;
                            existing.SwiftCode = bankDto.SwiftCode;
                            existing.Currency = bankDto.Currency;
                        }
                    }
                    else
                    {
                        _context.BankAccounts.Add(new BankAccount
                        {
                            TenantId = id,
                            IsPrimary = bankDto.IsPrimary,
                            BankName = bankDto.BankName,
                            AccountName = bankDto.AccountName,
                            AccountNumber = bankDto.AccountNumber,
                            Iban = bankDto.Iban,
                            SwiftCode = bankDto.SwiftCode,
                            Currency = bankDto.Currency
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            // Return the current RowVersion (it hasn't changed since we didn't touch Tenant)
            var currentRv = await query.AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => t.RowVersion)
                .FirstAsync();
            return currentRv;
        }

        public async Task<byte[]?> UpdateUsersAsync(Guid id, UpdateCompanyUsersDto dto)
        {
            var query = _context.Tenants.AsQueryable();
            if (_currentUserService.IsSuperAdmin) query = query.IgnoreQueryFilters();

            var tenantInfo = await query.AsNoTracking()
                .Where(t => t.Id == id && t.IsActive)
                .Select(t => new { t.RowVersion })
                .FirstOrDefaultAsync();

            if (tenantInfo == null) return null;

            if (!tenantInfo.RowVersion.SequenceEqual(dto.RowVersion))
            {
                throw new Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException("Another user has updated this record.");
            }

            // We need the tracked Tenant ONLY if Status is being updated
            if (!string.IsNullOrEmpty(dto.Status))
            {
                var tenant = await query.FirstOrDefaultAsync(t => t.Id == id && t.IsActive);
                if (tenant != null)
                {
                    tenant.Status = dto.Status;
                }
            }

            if (dto.UserTenantAccess != null)
            {
                var utaQuery = _context.UserTenantAccesses.AsQueryable();
                if (_currentUserService.IsSuperAdmin) utaQuery = utaQuery.IgnoreQueryFilters();

                var existingUta = await utaQuery
                    .Include(u => u.User)
                    .Where(u => u.TenantId == id)
                    .ToListAsync();

                var incomingUtaIds = dto.UserTenantAccess.Where(u => u.Id.HasValue).Select(u => u.Id.Value).ToList();
                var removedUta = existingUta.Where(u => !incomingUtaIds.Contains(u.Id)).ToList();

                _context.RemoveRange(removedUta);

                foreach (var utaDto in dto.UserTenantAccess)
                {
                    if (utaDto.Id.HasValue)
                    {
                        var existing = existingUta.FirstOrDefault(u => u.Id == utaDto.Id.Value);
                        if (existing != null)
                        {
                            existing.RoleId = utaDto.RoleId;
                            existing.IsActive = utaDto.IsActive;

                            if (existing.User != null)
                            {
                                if (!string.IsNullOrEmpty(utaDto.FirstName)) existing.User.FirstName = utaDto.FirstName;
                                if (utaDto.LastName != null) existing.User.LastName = utaDto.LastName;
                                
                                if (!string.IsNullOrEmpty(utaDto.Password))
                                {
                                    existing.User.PasswordHash = _passwordHasher.HashPassword(existing.User, utaDto.Password);
                                }
                            }
                        }
                    }
                    else
                    {
                        var userQuery = _context.Users.AsQueryable();
                        if (_currentUserService.IsSuperAdmin) userQuery = userQuery.IgnoreQueryFilters();

                        var user = await userQuery.FirstOrDefaultAsync(u => u.Email == utaDto.Email);
                        
                        if (user == null)
                        {
                            user = new User
                            {
                                TenantId = id, // Set the Home Tenant
                                Email = utaDto.Email,
                                FirstName = !string.IsNullOrEmpty(utaDto.FirstName) ? utaDto.FirstName : utaDto.Email.Split('@')[0], 
                                LastName = utaDto.LastName ?? "",

                                IsActive = true,
                                CreatedAt = DateTime.UtcNow
                            };

                            if (!string.IsNullOrEmpty(utaDto.Password))
                            {
                                user.PasswordHash = _passwordHasher.HashPassword(user, utaDto.Password);
                            }

                            _context.Users.Add(user);
                        }

                        _context.UserTenantAccesses.Add(new UserTenantAccess
                        {
                            TenantId = id,
                            User = user,
                            RoleId = utaDto.RoleId,
                            IsActive = utaDto.IsActive
                        });
                    }
                }
            }

            await _context.SaveChangesAsync();

            var currentRv = await query.AsNoTracking()
                .Where(t => t.Id == id)
                .Select(t => t.RowVersion)
                .FirstAsync();
            return currentRv;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var query = _context.Tenants.AsQueryable();
            if (_currentUserService.IsSuperAdmin) query = query.IgnoreQueryFilters();

            var tenant = await query.FirstOrDefaultAsync(t => t.Id == id && t.IsActive);

            if (tenant == null)
            {
                return false;
            }

            tenant.IsActive = false;
            tenant.ModifiedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return true;
        }
    }
}
