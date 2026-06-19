using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCCD_App.Migrations
{
    /// <inheritdoc />
    public partial class AddVolunteerTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VolunteerOpportunities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Committee = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequiredCount = table.Column<int>(type: "int", nullable: false),
                    Deadline = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerOpportunities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VolunteerApplications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OpportunityId = table.Column<int>(type: "int", nullable: false),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    Motivation = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Skills = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AppliedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VolunteerApplications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_VolunteerApplications_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VolunteerApplications_VolunteerOpportunities_OpportunityId",
                        column: x => x.OpportunityId,
                        principalTable: "VolunteerOpportunities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerApplications_OpportunityId",
                table: "VolunteerApplications",
                column: "OpportunityId");

            migrationBuilder.CreateIndex(
                name: "IX_VolunteerApplications_StudentId",
                table: "VolunteerApplications",
                column: "StudentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VolunteerApplications");

            migrationBuilder.DropTable(
                name: "VolunteerOpportunities");
        }
    }
}
