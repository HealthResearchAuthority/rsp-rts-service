using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rsp.RtsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRolesPK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrganisationRole",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole",
                columns: new[] { "Id", "OrganisationId", "Scoper", "Status" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "OrganisationRole",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddPrimaryKey(
                name: "PK_OrganisationRole",
                table: "OrganisationRole",
                columns: new[] { "Id", "OrganisationId", "Scoper" });
        }
    }
}
