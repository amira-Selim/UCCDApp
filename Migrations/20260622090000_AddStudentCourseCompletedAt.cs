using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCCD_App.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentCourseCompletedAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompletedAt",
                table: "StudentCourses",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompletedAt",
                table: "StudentCourses");
        }
    }
}
