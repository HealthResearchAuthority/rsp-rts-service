using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rsp.RtsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole");

            migrationBuilder.DropColumn(
                name: "CreatedDate",
                table: "OrganisationRole");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole",
                columns: new[] { "Id", "OrganisationId", "Scoper" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedDate",
                table: "OrganisationRole",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole",
                columns: new[] { "Id", "OrganisationId", "Scoper", "CreatedDate" });
        }
    }
}
