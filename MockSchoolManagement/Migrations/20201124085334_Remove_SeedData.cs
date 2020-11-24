using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MockSchoolManagement.Migrations
{
    public partial class Remove_SeedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Student",
                keyColumn: "Id",
                keyValue: 3);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Student",
                columns: new[] { "Id", "Email", "EnrollmentDate", "Major", "Name", "PhotoPath" },
                values: new object[] { 1, "648946942@qq.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "王鸿", null });

            migrationBuilder.InsertData(
                table: "Student",
                columns: new[] { "Id", "Email", "EnrollmentDate", "Major", "Name", "PhotoPath" },
                values: new object[] { 2, "474442945@qq.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, "朱海", null });

            migrationBuilder.InsertData(
                table: "Student",
                columns: new[] { "Id", "Email", "EnrollmentDate", "Major", "Name", "PhotoPath" },
                values: new object[] { 3, "906067946@qq.com", new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "林小康", null });
        }
    }
}
