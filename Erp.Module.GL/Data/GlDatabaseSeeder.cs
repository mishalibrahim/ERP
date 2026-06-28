using Erp.Module.GL.Entities;
using Erp.Shared.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Erp.Module.GL.Data
{
    public static class GlDatabaseSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<GlDbContext>();

            // Query core DB context (using EF query since we are in modular host where both are registered)
            // But since GlDbContext is multi-tenant and we are seeding on startup, we must bypass filters.
            // Let's get the tenant ID from the Tenants table. We can run raw SQL or if we don't have access to Core DbContext in GL,
            // we can retrieve it by looking at any entity or using a connection from the context.
            // Wait, we can query Core DB or we can query it using dbContext.Database connection.
            // Let's just find if we have any GlAccounts. If so, we are already seeded!
            if (await dbContext.GlAccounts.IgnoreQueryFilters().AnyAsync())
            {
                return; // Already seeded
            }

            // Get the Genesis Tenant ID by reading it from the database using raw SQL or ADO since we only have GlDbContext.
            // This is clean and doesn't require project references from GL back to Core (which would violate clean modular monolith rules).
            Guid? tenantId = null;
            using (var command = dbContext.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "SELECT TOP 1 Id FROM Tenants";
                await dbContext.Database.OpenConnectionAsync();
                using var reader = await command.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    tenantId = reader.GetGuid(0);
                }
                await dbContext.Database.CloseConnectionAsync();
            }

            if (tenantId == null)
            {
                Console.WriteLine("No Tenant found. GlDatabaseSeeder skipped.");
                return;
            }

            Guid actualTenantId = tenantId.Value;

            // ──────────────────────────────────────
            // 1. Seed Tax Groups
            // ──────────────────────────────────────
            var std5Tax = new TaxGroup
            {
                TenantId = actualTenantId,
                Name = "Standard 5%",
                Description = "Standard VAT rate of 5%"
            };
            var exemptTax = new TaxGroup
            {
                TenantId = actualTenantId,
                Name = "Exempt",
                Description = "VAT exempt goods and services"
            };
            var zeroTax = new TaxGroup
            {
                TenantId = actualTenantId,
                Name = "Zero-Rated",
                Description = "Zero-rated VAT of 0%"
            };

            dbContext.TaxGroups.AddRange(std5Tax, exemptTax, zeroTax);
            await dbContext.SaveChangesAsync();

            dbContext.TaxRates.AddRange(
                new TaxRate { TaxGroupId = std5Tax.Id, RatePercentage = 5.0000m, EffectiveFrom = new DateTime(2026, 1, 1), IsActive = true },
                new TaxRate { TaxGroupId = exemptTax.Id, RatePercentage = 0.0000m, EffectiveFrom = new DateTime(2026, 1, 1), IsActive = true },
                new TaxRate { TaxGroupId = zeroTax.Id, RatePercentage = 0.0000m, EffectiveFrom = new DateTime(2026, 1, 1), IsActive = true }
            );
            await dbContext.SaveChangesAsync();

            // ──────────────────────────────────────
            // 2. Seed Dimensions
            // ──────────────────────────────────────
            var costCenters = new List<Dimension>
            {
                new() { TenantId = actualTenantId, Type = DimensionType.CostCenter, Code = "Passport Services", Name = "Passport Services" },
                new() { TenantId = actualTenantId, Type = DimensionType.CostCenter, Code = "Travel & Tourism", Name = "Travel & Tourism" },
                new() { TenantId = actualTenantId, Type = DimensionType.CostCenter, Code = "Business Lounge", Name = "Business Lounge" },
                new() { TenantId = actualTenantId, Type = DimensionType.CostCenter, Code = "Admin", Name = "Admin" }
            };
            var departments = new List<Dimension>
            {
                new() { TenantId = actualTenantId, Type = DimensionType.Department, Code = "FIN", Name = "Finance" },
                new() { TenantId = actualTenantId, Type = DimensionType.Department, Code = "OPS", Name = "Operations" },
                new() { TenantId = actualTenantId, Type = DimensionType.Department, Code = "HR", Name = "HR" }
            };

            dbContext.Dimensions.AddRange(costCenters);
            dbContext.Dimensions.AddRange(departments);
            await dbContext.SaveChangesAsync();

            // ──────────────────────────────────────
            // 3. Seed Chart of Accounts
            // ──────────────────────────────────────
            
            // Assets
            var assetHeader = new GlAccount { TenantId = actualTenantId, AccountNumber = "100000", AccountName = "Assets", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Asset, PostingType = GlPostingType.Header, AllowManualEntry = false };
            var bankHeader = new GlAccount { TenantId = actualTenantId, AccountNumber = "110000", AccountName = "Cash & Bank Accounts", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Asset, PostingType = GlPostingType.Header, AllowManualEntry = false };
            
            dbContext.GlAccounts.AddRange(assetHeader, bankHeader);
            await dbContext.SaveChangesAsync();

            // Child accounts
            var a1 = new GlAccount { TenantId = actualTenantId, ParentAccountId = bankHeader.Id, AccountNumber = "110100", AccountName = "Cash in Hand", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Asset, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var a2 = new GlAccount { TenantId = actualTenantId, ParentAccountId = bankHeader.Id, AccountNumber = "110200", AccountName = "ENBD Current Account", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Asset, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var a3 = new GlAccount { TenantId = actualTenantId, ParentAccountId = bankHeader.Id, AccountNumber = "110300", AccountName = "ADCB Savings Account", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Asset, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            
            var receivableHeader = new GlAccount { TenantId = actualTenantId, ParentAccountId = assetHeader.Id, AccountNumber = "120000", AccountName = "Receivables", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Asset, PostingType = GlPostingType.Header, AllowManualEntry = false };
            dbContext.GlAccounts.AddRange(a1, a2, a3, receivableHeader);
            await dbContext.SaveChangesAsync();

            var a4 = new GlAccount { TenantId = actualTenantId, ParentAccountId = receivableHeader.Id, AccountNumber = "120100", AccountName = "Accounts Receivable", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Asset, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var a5 = new GlAccount { TenantId = actualTenantId, ParentAccountId = assetHeader.Id, AccountNumber = "130100", AccountName = "Prepaid Expenses", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Asset, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var a6 = new GlAccount { TenantId = actualTenantId, ParentAccountId = assetHeader.Id, AccountNumber = "140100", AccountName = "Office Equipment", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Asset, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            
            // Liabilities
            var liabHeader = new GlAccount { TenantId = actualTenantId, AccountNumber = "200000", AccountName = "Liabilities", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Liability, PostingType = GlPostingType.Header, AllowManualEntry = false };
            var payHeader = new GlAccount { TenantId = actualTenantId, AccountNumber = "210000", AccountName = "Current Liabilities", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Liability, PostingType = GlPostingType.Header, AllowManualEntry = false };
            dbContext.GlAccounts.AddRange(a4, a5, a6, liabHeader, payHeader);
            await dbContext.SaveChangesAsync();

            var l1 = new GlAccount { TenantId = actualTenantId, ParentAccountId = payHeader.Id, AccountNumber = "210100", AccountName = "Accounts Payable", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Liability, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var l2 = new GlAccount { TenantId = actualTenantId, ParentAccountId = payHeader.Id, AccountNumber = "210200", AccountName = "Accrued Expenses", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Liability, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var l3 = new GlAccount { TenantId = actualTenantId, ParentAccountId = liabHeader.Id, AccountNumber = "220100", AccountName = "VAT Payable", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Liability, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var l4 = new GlAccount { TenantId = actualTenantId, ParentAccountId = liabHeader.Id, AccountNumber = "230100", AccountName = "Short-term Loan - ENBD", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Liability, PostingType = GlPostingType.Posting, AllowManualEntry = true };

            // Equity
            var eqHeader = new GlAccount { TenantId = actualTenantId, AccountNumber = "300000", AccountName = "Equity", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Equity, PostingType = GlPostingType.Header, AllowManualEntry = false };
            dbContext.GlAccounts.AddRange(l1, l2, l3, l4, eqHeader);
            await dbContext.SaveChangesAsync();

            var e1 = new GlAccount { TenantId = actualTenantId, ParentAccountId = eqHeader.Id, AccountNumber = "310100", AccountName = "Share Capital", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Equity, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var e2 = new GlAccount { TenantId = actualTenantId, ParentAccountId = eqHeader.Id, AccountNumber = "320100", AccountName = "Retained Earnings", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Equity, PostingType = GlPostingType.Posting, AllowManualEntry = true };

            // Revenue
            var revHeader = new GlAccount { TenantId = actualTenantId, AccountNumber = "400000", AccountName = "Revenue", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Income, PostingType = GlPostingType.Header, AllowManualEntry = false };
            var opRevHeader = new GlAccount { TenantId = actualTenantId, AccountNumber = "410000", AccountName = "Operating Revenue", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Income, PostingType = GlPostingType.Header, AllowManualEntry = false };
            dbContext.GlAccounts.AddRange(e1, e2, revHeader, opRevHeader);
            await dbContext.SaveChangesAsync();

            var r1 = new GlAccount { TenantId = actualTenantId, ParentAccountId = opRevHeader.Id, AccountNumber = "410100", AccountName = "Passport Service Revenue", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Income, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var r2 = new GlAccount { TenantId = actualTenantId, ParentAccountId = opRevHeader.Id, AccountNumber = "410200", AccountName = "Travel & Tourism Revenue", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Income, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var r3 = new GlAccount { TenantId = actualTenantId, ParentAccountId = opRevHeader.Id, AccountNumber = "410300", AccountName = "Business Lounge Revenue", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Income, PostingType = GlPostingType.Posting, AllowManualEntry = true };

            // Expenses
            var expHeader = new GlAccount { TenantId = actualTenantId, AccountNumber = "500000", AccountName = "Expenses", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Expense, PostingType = GlPostingType.Header, AllowManualEntry = false };
            var opExpHeader = new GlAccount { TenantId = actualTenantId, AccountNumber = "510000", AccountName = "Operating Expenses", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Expense, PostingType = GlPostingType.Header, AllowManualEntry = false };
            dbContext.GlAccounts.AddRange(r1, r2, r3, expHeader, opExpHeader);
            await dbContext.SaveChangesAsync();

            var x1 = new GlAccount { TenantId = actualTenantId, ParentAccountId = opExpHeader.Id, AccountNumber = "510100", AccountName = "Salaries & Wages", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Expense, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var x2 = new GlAccount { TenantId = actualTenantId, ParentAccountId = opExpHeader.Id, AccountNumber = "510200", AccountName = "Rent Expense", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Expense, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var x3 = new GlAccount { TenantId = actualTenantId, ParentAccountId = opExpHeader.Id, AccountNumber = "510300", AccountName = "Utilities Expense", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Expense, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var x4 = new GlAccount { TenantId = actualTenantId, ParentAccountId = opExpHeader.Id, AccountNumber = "510400", AccountName = "Office Supplies", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Expense, PostingType = GlPostingType.Posting, AllowManualEntry = true };
            var x5 = new GlAccount { TenantId = actualTenantId, ParentAccountId = opExpHeader.Id, AccountNumber = "510500", AccountName = "Marketing Expense", AccountType = GlAccountType.Ledger, AccountCategory = GlAccountCategory.Expense, PostingType = GlPostingType.Posting, AllowManualEntry = true };

            dbContext.GlAccounts.AddRange(x1, x2, x3, x4, x5);
            await dbContext.SaveChangesAsync();
        }
    }
}
