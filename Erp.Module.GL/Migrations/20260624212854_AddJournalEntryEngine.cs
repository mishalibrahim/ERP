using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp.Module.GL.Migrations
{
    /// <inheritdoc />
    public partial class AddJournalEntryEngine : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "RatePercentage",
                table: "TaxRates",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "JournalEntryLines",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "JournalEntryLines",
                type: "decimal(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)");

            migrationBuilder.AddColumn<Guid>(
                name: "DefaultTaxGroupId",
                table: "GlAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlAccounts_DefaultTaxGroupId",
                table: "GlAccounts",
                column: "DefaultTaxGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlAccounts_TaxGroups_DefaultTaxGroupId",
                table: "GlAccounts",
                column: "DefaultTaxGroupId",
                principalTable: "TaxGroups",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlAccounts_TaxGroups_DefaultTaxGroupId",
                table: "GlAccounts");

            migrationBuilder.DropIndex(
                name: "IX_GlAccounts_DefaultTaxGroupId",
                table: "GlAccounts");

            migrationBuilder.DropColumn(
                name: "DefaultTaxGroupId",
                table: "GlAccounts");

            migrationBuilder.AlterColumn<decimal>(
                name: "RatePercentage",
                table: "TaxRates",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Debit",
                table: "JournalEntryLines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);

            migrationBuilder.AlterColumn<decimal>(
                name: "Credit",
                table: "JournalEntryLines",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,4)",
                oldPrecision: 18,
                oldScale: 4);
        }
    }
}
