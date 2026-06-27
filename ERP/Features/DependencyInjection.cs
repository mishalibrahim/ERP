using ERP.Features.Auth;
using ERP.Features.CompanySetup;
using ERP.Features.Roles;
using ERP.Features.Dimensions;
using ERP.Features.GlAccounts;
using ERP.Features.Taxes;
using ERP.Features.JournalEntries;
using ERP.Features.GeneralLedger;
using Erp.Module.Core.Entities;
using Erp.Shared.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace ERP.Features
{
    public static class FeatureServiceExtensions
    {
        public static IServiceCollection AddFeatureServices(this IServiceCollection services)
        {
            // Auth
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            // Company Setup
            services.AddScoped<ICompanySetupService, CompanySetupService>();

            // Roles
            services.AddScoped<IRoleService, RoleService>();

            // GL Module Features
            services.AddScoped<IGlAccountService, GlAccountService>();
            services.AddScoped<ITaxService, TaxService>();
            services.AddScoped<IDimensionService, DimensionService>();
            services.AddScoped<IJournalEntryService, JournalEntryService>();
            services.AddScoped<IGeneralLedgerService, GeneralLedgerService>();

            return services;
        }
    }
}
