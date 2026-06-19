using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCCD_App.Migrations
{
    /// <inheritdoc />
    public partial class AddAdditionalProfileFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CareerGoal",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Education",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Interests",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Skills",
                table: "Students",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CareerGoal",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Education",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Interests",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Skills",
                table: "Students");
        }
    }
}
