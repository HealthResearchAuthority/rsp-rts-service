using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rsp.RtsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveTermsetTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Organisation_OrganisationTermset_Type",
                table: "Organisation");

            migrationBuilder.DropTable(
                name: "OrganisationTermset");

            migrationBuilder.DropIndex(
                name: "IX_Organisation_Type",
                table: "Organisation");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "OrganisationTermset",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(150)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Imported = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationTermset", x => x.Id);
                });

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
    }
}
