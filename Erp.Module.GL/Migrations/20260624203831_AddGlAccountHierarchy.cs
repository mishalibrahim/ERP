using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Erp.Module.GL.Migrations
{
    /// <inheritdoc />
    public partial class AddGlAccountHierarchy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ParentAccountId",
                table: "GlAccounts",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GlAccounts_ParentAccountId",
                table: "GlAccounts",
                column: "ParentAccountId");

            migrationBuilder.AddForeignKey(
                name: "FK_GlAccounts_GlAccounts_ParentAccountId",
                table: "GlAccounts",
                column: "ParentAccountId",
                principalTable: "GlAccounts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GlAccounts_GlAccounts_ParentAccountId",
                table: "GlAccounts");

            migrationBuilder.DropIndex(
                name: "IX_GlAccounts_ParentAccountId",
                table: "GlAccounts");

            migrationBuilder.DropColumn(
                name: "ParentAccountId",
                table: "GlAccounts");
        }
    }
}
