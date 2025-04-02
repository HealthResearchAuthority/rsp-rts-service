using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rsp.RtsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateOrganisationFieldTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<bool>(
                name: "Status",
                table: "Organisation",
                type: "bit",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "OId",
                table: "Organisation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeId",
                table: "Organisation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "TypeName",
                table: "Organisation",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OId",
                table: "Organisation");

            migrationBuilder.DropColumn(
                name: "TypeId",
                table: "Organisation");

            migrationBuilder.DropColumn(
                name: "TypeName",
                table: "Organisation");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Organisation",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
