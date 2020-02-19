using Microsoft.EntityFrameworkCore.Migrations;

namespace AttendanceSystemIPCamera.Migrations
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Active",
                table: "Sessions",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Groups",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Attendees",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Active",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Groups");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Attendees");
        }
    }
}
