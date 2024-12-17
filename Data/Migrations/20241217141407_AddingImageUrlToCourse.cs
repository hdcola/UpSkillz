using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UpSkillz.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingImageUrlToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "imageUrl",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "5f130bc0-0063-4d17-bbff-82067ed52300", null, "User", "USER" },
                    { "93c12aac-fbed-4a4a-943e-e789e9073000", null, "Admin", "ADMIN" },
                    { "b69525fd-b6f8-4cbf-ad66-b8b3352da510", null, "Instructor", "INSTRUCTOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "5f130bc0-0063-4d17-bbff-82067ed52300");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "93c12aac-fbed-4a4a-943e-e789e9073000");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b69525fd-b6f8-4cbf-ad66-b8b3352da510");

            migrationBuilder.DropColumn(
                name: "imageUrl",
                table: "Courses");

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
    }
}
