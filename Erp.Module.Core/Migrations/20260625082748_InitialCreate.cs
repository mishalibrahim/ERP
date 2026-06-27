using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp.Module.Core.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permissions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Module = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Action = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Resource = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenants",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TradeName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CompanyCode = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    LicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Emirate = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlaceOfIncorporation = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsFreeZoneEntity = table.Column<bool>(type: "bit", nullable: false),
                    IsDesignatedZone = table.Column<bool>(type: "bit", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Financials_FinancialYearStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Financials_FinancialYearEnd = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Financials_BooksStartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Financials_AccountingMethod = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Financials_FiscalYear = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Financials_BaseCurrency = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Financials_ReportingCurrency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Localization_OrganizationLanguage = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Localization_CommunicationLanguages = table.Column<string>(type: "nvarchar(max)", nullable: false),
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
                    VatDetails_FirstVatPeriod = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VatDetails_VatReturnStartPeriod = table.Column<DateTime>(type: "datetime2", nullable: true),
                    VatDetails_VatDeregistrationDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CorporateTax_CtRegistered = table.Column<bool>(type: "bit", nullable: false),
                    CorporateTax_CorporateTaxTrn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CorporateTax_FirstTaxPeriodStart = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CorporateTax_FreeZonePerson = table.Column<bool>(type: "bit", nullable: false),
                    CorporateTax_QfzpStatus = table.Column<bool>(type: "bit", nullable: false),
                    CorporateTax_SmallBusinessRelief = table.Column<bool>(type: "bit", nullable: false),
                    TaxConfiguration_DefaultVatRateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TaxConfiguration_InputVatAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TaxConfiguration_OutputVatAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Controls_MultiCompanyEnable = table.Column<bool>(type: "bit", nullable: false),
                    Controls_AuditTrailEnable = table.Column<bool>(type: "bit", nullable: false),
                    Controls_ApprovalWorkflow = table.Column<bool>(type: "bit", nullable: false),
                    Controls_DefaultCostCenterId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Controls_DefaultProjectId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents_TradeLicenseUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents_MoaUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents_VatCertificateUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents_EmiratesIdUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Documents_PassportCopyUrl = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenants", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankAccounts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
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
                    table.PrimaryKey("PK_BankAccounts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankAccounts_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DocumentNumberSeries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DocumentType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Prefix = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CurrentNumber = table.Column<long>(type: "bigint", nullable: false),
                    Suffix = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DocumentNumberSeries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DocumentNumberSeries_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PostingGroups",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ReceivablesAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PayablesAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InventoryAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CogsAccountId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PostingGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PostingGroups_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemRole = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Roles_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirstName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ModifiedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RolePermissions",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PermissionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GrantedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RolePermissions", x => new { x.RoleId, x.PermissionId });
                    table.ForeignKey(
                        name: "FK_RolePermissions_Permissions_PermissionId",
                        column: x => x.PermissionId,
                        principalTable: "Permissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RolePermissions_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserTenantAccesses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RoleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserTenantAccesses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserTenantAccesses_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTenantAccesses_Tenants_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenants",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserTenantAccesses_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankAccounts_TenantId",
                table: "BankAccounts",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_DocumentNumberSeries_TenantId",
                table: "DocumentNumberSeries",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Permissions_Module_Action_Resource",
                table: "Permissions",
                columns: new[] { "Module", "Action", "Resource" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PostingGroups_TenantId",
                table: "PostingGroups",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RolePermissions_PermissionId",
                table: "RolePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_TenantId",
                table: "Roles",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_Tenants_CompanyCode",
                table: "Tenants",
                column: "CompanyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_TenantId",
                table: "Users",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTenantAccesses_RoleId",
                table: "UserTenantAccesses",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTenantAccesses_TenantId",
                table: "UserTenantAccesses",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserTenantAccesses_UserId",
                table: "UserTenantAccesses",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankAccounts");

            migrationBuilder.DropTable(
                name: "DocumentNumberSeries");

            migrationBuilder.DropTable(
                name: "PostingGroups");

            migrationBuilder.DropTable(
                name: "RolePermissions");

            migrationBuilder.DropTable(
                name: "UserTenantAccesses");

            migrationBuilder.DropTable(
                name: "Permissions");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Tenants");
        }
    }
}
