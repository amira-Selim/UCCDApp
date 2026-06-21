using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCCD_App.Migrations
{
    /// <inheritdoc />
    public partial class AddCourseInstructorAndEnrollmentDate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Instructor",
                table: "Courses",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EnrolledAt",
                table: "StudentCourses",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Instructor",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "EnrolledAt",
                table: "StudentCourses");
        }
    }
}
