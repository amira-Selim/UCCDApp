using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCCD_App.Migrations
{
    /// <inheritdoc />
    public partial class JobStatusAndRejectionReason : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsApproved",
                table: "JobOpportunities");

            migrationBuilder.AddColumn<string>(
                name: "RejectionReason",
                table: "JobOpportunities",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "JobOpportunities",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RejectionReason",
                table: "JobOpportunities");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "JobOpportunities");

            migrationBuilder.AddColumn<bool>(
                name: "IsApproved",
                table: "JobOpportunities",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
