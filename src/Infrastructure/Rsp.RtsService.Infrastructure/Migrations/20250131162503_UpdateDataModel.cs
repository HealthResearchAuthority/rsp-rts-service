using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rsp.RtsService.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDataModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Organisation",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(150)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CountryIdentifier = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Type = table.Column<string>(type: "varchar(150)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Imported = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organisation", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationTermset",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(150)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Imported = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationTermset", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "OrganisationRole",
                columns: table => new
                {
                    Id = table.Column<string>(type: "varchar(150)", nullable: false),
                    OrganisationId = table.Column<string>(type: "varchar(150)", nullable: false),
                    Scoper = table.Column<string>(type: "varchar(150)", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SystemUpdated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Imported = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganisationRole", x => new { x.Id, x.OrganisationId, x.Scoper, x.StartDate });
                    table.ForeignKey(
                        name: "FK_OrganisationRole_Organisation_OrganisationId",
                        column: x => x.OrganisationId,
                        principalTable: "Organisation",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrganisationRole_OrganisationId",
                table: "OrganisationRole",
                column: "OrganisationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganisationRole");

            migrationBuilder.DropTable(
                name: "OrganisationTermset");

            migrationBuilder.DropTable(
                name: "Organisation");
        }
    }
}
