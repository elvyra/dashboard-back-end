using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dashboard.Data.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Portals",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Type = table.Column<int>(nullable: false),
                    URL = table.Column<string>(nullable: true),
                    Parameters = table.Column<string>(nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    CheckInterval = table.Column<int>(nullable: false),
                    Method = table.Column<int>(nullable: false),
                    BasicAuth = table.Column<bool>(nullable: false),
                    UserName = table.Column<string>(nullable: true),
                    PasswordHashed = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Portals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PortalResponses",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    RequestDateTime = table.Column<DateTime>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    ResponseTime = table.Column<int>(nullable: false),
                    StatusPageId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PortalResponses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PortalResponses_Portals_StatusPageId",
                        column: x => x.StatusPageId,
                        principalTable: "Portals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PortalResponses_StatusPageId",
                table: "PortalResponses",
                column: "StatusPageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PortalResponses");

            migrationBuilder.DropTable(
                name: "Portals");
        }
    }
}
