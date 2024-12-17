using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UpSkillz.Data.Migrations
{
    /// <inheritdoc />
    public partial class TrasferingToNewDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "74dd4210-f836-42cd-9ee1-12eaa24bb84f");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "76e273b6-0bad-4a9f-9eef-067d54664ead");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "fe74de0c-8b42-4559-82c6-618b57d03b91");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "269d90cd-abcc-4ec9-b410-bad32534bfb2", null, "Admin", "ADMIN" },
                    { "a23f9b61-f6be-488b-8a9f-310f2cdc2c22", null, "Instructor", "INSTRUCTOR" },
                    { "f8a1ca57-dcd4-41d3-aaf6-984b3a33012f", null, "User", "USER" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "269d90cd-abcc-4ec9-b410-bad32534bfb2");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "a23f9b61-f6be-488b-8a9f-310f2cdc2c22");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "f8a1ca57-dcd4-41d3-aaf6-984b3a33012f");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "74dd4210-f836-42cd-9ee1-12eaa24bb84f", null, "Instructor", "INSTRUCTOR" },
                    { "76e273b6-0bad-4a9f-9eef-067d54664ead", null, "User", "USER" },
                    { "fe74de0c-8b42-4559-82c6-618b57d03b91", null, "Admin", "ADMIN" }
                });
        }
    }
}
