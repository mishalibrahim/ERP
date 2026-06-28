using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp.Module.GL.Migrations
{
    /// <inheritdoc />
    public partial class AddJournalVoucherFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // ── JournalEntries: new Journal Voucher fields ──────────────────────
            migrationBuilder.AddColumn<string>(
                name: "VoucherNo",
                table: "JournalEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "JournalName",
                table: "JournalEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Currency",
                table: "JournalEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "AED");

            migrationBuilder.AddColumn<int>(
                name: "JournalType",
                table: "JournalEntries",
                type: "int",
                nullable: false,
                defaultValue: 0); // General

            migrationBuilder.AddColumn<string>(
                name: "CostCenter",
                table: "JournalEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Department",
                table: "JournalEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ExchangeRate",
                table: "JournalEntries",
                type: "decimal(18,6)",
                nullable: false,
                defaultValue: 1.0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "JournalEntries",
                type: "int",
                nullable: false,
                defaultValue: 0); // Draft

            migrationBuilder.AddColumn<string>(
                name: "InternalNotes",
                table: "JournalEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalRemarks",
                table: "JournalEntries",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CurrentApprovalStage",
                table: "JournalEntries",
                type: "int",
                nullable: false,
                defaultValue: 0); // Initiator

            migrationBuilder.AddColumn<string>(
                name: "ApprovalHistoryJson",
                table: "JournalEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<string>(
                name: "AttachmentsJson",
                table: "JournalEntries",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AddColumn<Guid>(
                name: "ReversedVoucherId",
                table: "JournalEntries",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ReversingVoucherId",
                table: "JournalEntries",
                type: "uniqueidentifier",
                nullable: true);

            // ── JournalEntryLines: new extended line fields ─────────────────────
            migrationBuilder.AddColumn<int>(
                name: "AccountType",
                table: "JournalEntryLines",
                type: "int",
                nullable: false,
                defaultValue: 0); // Ledger

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "JournalEntryLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CostCenter",
                table: "JournalEntryLines",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "OffsetType",
                table: "JournalEntryLines",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "OffsetAccountId",
                table: "JournalEntryLines",
                type: "uniqueidentifier",
                nullable: true);

            // ── Foreign keys & indexes ───────────────────────────────────────────
            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_ReversedVoucherId",
                table: "JournalEntries",
                column: "ReversedVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntries_ReversingVoucherId",
                table: "JournalEntries",
                column: "ReversingVoucherId");

            migrationBuilder.CreateIndex(
                name: "IX_JournalEntryLines_OffsetAccountId",
                table: "JournalEntryLines",
                column: "OffsetAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_JournalEntries_ReversedVoucherId",
                table: "JournalEntries",
                column: "ReversedVoucherId",
                principalTable: "JournalEntries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntries_JournalEntries_ReversingVoucherId",
                table: "JournalEntries",
                column: "ReversingVoucherId",
                principalTable: "JournalEntries",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_JournalEntryLines_GlAccounts_OffsetAccountId",
                table: "JournalEntryLines",
                column: "OffsetAccountId",
                principalTable: "GlAccounts",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_JournalEntries_ReversedVoucherId",
                table: "JournalEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntries_JournalEntries_ReversingVoucherId",
                table: "JournalEntries");

            migrationBuilder.DropForeignKey(
                name: "FK_JournalEntryLines_GlAccounts_OffsetAccountId",
                table: "JournalEntryLines");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_ReversedVoucherId",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntries_ReversingVoucherId",
                table: "JournalEntries");

            migrationBuilder.DropIndex(
                name: "IX_JournalEntryLines_OffsetAccountId",
                table: "JournalEntryLines");

            migrationBuilder.DropColumn(name: "VoucherNo", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "JournalName", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "Currency", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "JournalType", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "CostCenter", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "Department", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "ExchangeRate", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "Status", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "InternalNotes", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "ApprovalRemarks", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "CurrentApprovalStage", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "ApprovalHistoryJson", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "AttachmentsJson", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "ReversedVoucherId", table: "JournalEntries");
            migrationBuilder.DropColumn(name: "ReversingVoucherId", table: "JournalEntries");

            migrationBuilder.DropColumn(name: "AccountType", table: "JournalEntryLines");
            migrationBuilder.DropColumn(name: "Description", table: "JournalEntryLines");
            migrationBuilder.DropColumn(name: "CostCenter", table: "JournalEntryLines");
            migrationBuilder.DropColumn(name: "OffsetType", table: "JournalEntryLines");
            migrationBuilder.DropColumn(name: "OffsetAccountId", table: "JournalEntryLines");
        }
    }
}