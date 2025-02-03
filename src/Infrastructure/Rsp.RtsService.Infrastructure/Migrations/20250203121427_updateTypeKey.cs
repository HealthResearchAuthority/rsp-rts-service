using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rsp.RtsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class updateTypeKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Organisation_Type",
                table: "Organisation",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_Organisation_OrganisationTermset_Type",
                table: "Organisation",
                column: "Type",
                principalTable: "OrganisationTermset",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_OrganisationTermset_Type",
                table: "Organisation");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_Type",
                table: "Organisation");
        }
    }
}
