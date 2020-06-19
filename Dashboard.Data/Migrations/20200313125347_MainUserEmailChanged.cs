using Microsoft.EntityFrameworkCore.Migrations;

namespace Dashboard.Data.Migrations
{
    public partial class MainUserEmailChanged : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1,
                columns: new[] { "Email", "Password" },
                values: new object[] { "admin@admin.com", "$MYHASH$V1$10000$GpLl/x02c5yxGYzXRtNJjXkxLKD0UW6r0nRE4Fy2Uk7jKPmR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1,
                columns: new[] { "Email", "Password" },
                values: new object[] { "admin@admin", "$MYHASH$V1$10000$DMH+wqzgsC+hs0MJBtlR+f0GsJG7c+TIQjrbBar59uWO17R5" });
        }
    }
}
