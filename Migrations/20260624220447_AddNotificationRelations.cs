using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UCCD_App.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RelatedJobId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RelatedVolunteerId",
                table: "Notifications",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedCourseId",
                table: "Notifications",
                column: "RelatedCourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedJobId",
                table: "Notifications",
                column: "RelatedJobId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_RelatedVolunteerId",
                table: "Notifications",
                column: "RelatedVolunteerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_Courses_RelatedCourseId",
                table: "Notifications",
                column: "RelatedCourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_JobOpportunities_RelatedJobId",
                table: "Notifications",
                column: "RelatedJobId",
                principalTable: "JobOpportunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Notifications_VolunteerOpportunities_RelatedVolunteerId",
                table: "Notifications",
                column: "RelatedVolunteerId",
                principalTable: "VolunteerOpportunities",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_Courses_RelatedCourseId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_JobOpportunities_RelatedJobId",
                table: "Notifications");

            migrationBuilder.DropForeignKey(
                name: "FK_Notifications_VolunteerOpportunities_RelatedVolunteerId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_RelatedCourseId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_RelatedJobId",
                table: "Notifications");

            migrationBuilder.DropIndex(
                name: "IX_Notifications_RelatedVolunteerId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RelatedJobId",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "RelatedVolunteerId",
                table: "Notifications");
        }
    }
}
