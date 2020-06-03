using Microsoft.EntityFrameworkCore.Migrations;

namespace AttendanceSystemIPCamera.Migrations
{
    public partial class Update1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChangeRequests",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttendeeId = table.Column<int>(nullable: true),
                    RecordId = table.Column<int>(nullable: true),
                    Comment = table.Column<string>(nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChangeRequests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Attendees_AttendeeId",
                        column: x => x.AttendeeId,
                        principalTable: "Attendees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ChangeRequests_Records_RecordId",
                        column: x => x.RecordId,
                        principalTable: "Records",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.AddColumn<bool>(
                name: "NewState",
                table: "ChangeRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "OldState",
                table: "ChangeRequests",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ChangeRequests",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NewState",
                table: "ChangeRequests");

            migrationBuilder.DropColumn(
                name: "OldState",
                table: "ChangeRequests");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ChangeRequests");

            migrationBuilder.AddColumn<bool>(
                name: "Present",
                table: "ChangeRequests",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }
    }
}
