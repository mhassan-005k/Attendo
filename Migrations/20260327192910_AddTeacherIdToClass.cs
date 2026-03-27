using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AttendanceApp.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherIdToClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Classes");
        }
    }
}
