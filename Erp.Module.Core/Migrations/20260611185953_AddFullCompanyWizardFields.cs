using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp.Module.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddFullCompanyWizardFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TradeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Emirate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsFreeZoneEntity = table.Column<bool>(type: "bit", nullable: false),
                    IsDesignatedZone = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Financials_FinancialYearStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Financials_BooksStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Financials_AccountingMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Financials_FiscalYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Financials_BaseCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Financials_ReportingCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Localization_OrganizationLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Localization_InvoiceLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Localization_TimeZone = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Localization_DateFormat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredAddress_AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredAddress_AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RegisteredAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredAddress_Emirate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredAddress_POBox = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredAddress_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegisteredAddress_FaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddress_AddressLine1 = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingAddress_AddressLine2 = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BillingAddress_City = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingAddress_Emirate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingAddress_POBox = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingAddress_Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingAddress_PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    BillingAddress_FaxNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VatDetails_VatRegistered = table.Column<bool>(type: "bit", nullable: false),
                    VatDetails_TrnLabel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VatDetails_TrnNumber = table.Column<string>(type: "nvarchar(15)", maxLength: 15, nullable: true),
                    VatDetails_VatScheme = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VatDetails_FilingFrequency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VatDetails_VatRegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CorporateTax_CtRegistered = table.Column<bool>(type: "bit", nullable: false),
                    CorporateTax_CorporateTaxTrn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorporateTax_FirstTaxPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CorporateTax_FreeZonePerson = table.Column<bool>(type: "bit", nullable: false),
                    CorporateTax_QfzpStatus = table.Column<bool>(type: "bit", nullable: false),
                    CorporateTax_SmallBusinessRelief = table.Column<bool>(type: "bit", nullable: false),
                    Controls_MultiCompanyEnable = table.Column<bool>(type: "bit", nullable: false),
                    Controls_AuditTrailEnable = table.Column<bool>(type: "bit", nullable: false),
                    Controls_ApprovalWorkflow = table.Column<bool>(type: "bit", nullable: false),
                    Documents_TradeLicenseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents_MoaUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents_VatCertificateUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents_EmiratesIdUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents_PassportCopyUrl = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccount",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IsPrimary = table.Column<bool>(type: "bit", nullable: false),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Iban = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SwiftCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankAccount", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccount_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccount_TenantId",
                table: "BankAccount",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CompanyCode",
                table: "Tenants",
                column: "CompanyCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccount");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
