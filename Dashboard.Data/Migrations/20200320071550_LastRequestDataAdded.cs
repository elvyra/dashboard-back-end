using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Dashboard.Data.Migrations
{
    public partial class LastRequestDataAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastRequestDateTime",
                table: "Portals",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "LastRequestErrorMessage",
                table: "Portals",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LastRequestResponseTime",
                table: "Portals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "LastRequestStatus",
                table: "Portals",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1,
                column: "Password",
                value: "$MYHASH$V1$10000$4/Rsx+rg2UU0pqd+bRkRJFI1EyhfgJNZ0Ae8n1s3/aqIrW2S");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastRequestDateTime",
                table: "Portals");

            migrationBuilder.DropColumn(
                name: "LastRequestErrorMessage",
                table: "Portals");

            migrationBuilder.DropColumn(
                name: "LastRequestResponseTime",
                table: "Portals");

            migrationBuilder.DropColumn(
                name: "LastRequestStatus",
                table: "Portals");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1,
                column: "Password",
                value: "$MYHASH$V1$10000$Yb5M+8ZjETs8o8AxeFj+Ng6pwO3lXE1gJe2WkCJPiKI20dLA");
        }
    }
}
