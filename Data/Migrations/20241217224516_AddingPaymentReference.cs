using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace UpSkillz.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddingPaymentReference : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.AddColumn<string>(
                name: "PaymentReference",
                table: "Enrollments",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "29f6602d-21a2-49a7-a520-bc8f73b0a3c8", null, "Admin", "ADMIN" },
                    { "9fb25e83-1d16-4f79-b633-5a208931d69a", null, "User", "USER" },
                    { "b0c275c1-6909-434e-937b-1ccae22af564", null, "Instructor", "INSTRUCTOR" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "29f6602d-21a2-49a7-a520-bc8f73b0a3c8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "9fb25e83-1d16-4f79-b633-5a208931d69a");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "b0c275c1-6909-434e-937b-1ccae22af564");

            migrationBuilder.DropColumn(
                name: "PaymentReference",
                table: "Enrollments");

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
    }
}
