using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rsp.RtsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class changeRolesKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "OrganisationRole",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "OrganisationRole",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole",
                columns: new[] { "Id", "OrganisationId", "Scoper", "CreatedDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "OrganisationRole",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedDate",
                table: "OrganisationRole",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole",
                columns: new[] { "Id", "OrganisationId", "Scoper", "StartDate" });
        }
    }
}
