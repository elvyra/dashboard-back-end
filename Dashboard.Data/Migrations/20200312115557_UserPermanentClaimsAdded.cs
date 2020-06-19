using Microsoft.EntityFrameworkCore.Migrations;

namespace Dashboard.Data.Migrations
{
    public partial class UserPermanentClaimsAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ProtectedUsers");

            migrationBuilder.AddColumn<string>(
                name: "Claims",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "isPermanent",
                table: "Users",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "Claims", "Email", "IsActive", "Name", "Password", "Surname", "isPermanent" },
                values: new object[] { -1, "isAdmin", "admin@admin", true, "Main", "$MYHASH$V1$10000$DMH+wqzgsC+hs0MJBtlR+f0GsJG7c+TIQjrbBar59uWO17R5", "User", true });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: -1);

            migrationBuilder.DropColumn(
                name: "Claims",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "isPermanent",
                table: "Users");

            migrationBuilder.CreateTable(
                name: "ProtectedUsers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProtectedUsers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProtectedUsers_Users_Id",
                        column: x => x.Id,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });
        }
    }
}
