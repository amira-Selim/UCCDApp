using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCCD_App.Migrations
{
    /// <inheritdoc />
    public partial class VaryJobTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE JobOpportunities SET Type = 1 WHERE Id % 4 = 1;"); // PartTime
            migrationBuilder.Sql("UPDATE JobOpportunities SET Type = 2 WHERE Id % 4 = 2;"); // Internship
            migrationBuilder.Sql("UPDATE JobOpportunities SET Type = 3 WHERE Id % 4 = 3;"); // Freelance
            migrationBuilder.Sql("UPDATE JobOpportunities SET Type = 0 WHERE Id % 4 = 0;"); // FullTime
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
