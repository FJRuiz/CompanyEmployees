using Microsoft.EntityFrameworkCore.Migrations;

namespace CompanyEmployees.Migrations
{
    public partial class AddRolesToDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "92cf4022-85ec-4515-9317-4bb88ba2641d", "37fc826d-d7f6-49b8-b6a6-f02dc7ae6f33", "Manager", "MANAGER" });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "ab2733bd-db87-4ad4-9493-7336816b7e5c", "ac695327-13cb-4c47-9aab-716b5583b1b0", "Administrador", "ADMINISTRADOR" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "92cf4022-85ec-4515-9317-4bb88ba2641d");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "ab2733bd-db87-4ad4-9493-7336816b7e5c");
        }
    }
}
