using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AnimalAnnouncementReportCountMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ReportCount",
                table: "AnimalAnnouncements",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.Sql("""
                                 UPDATE "AnimalAnnouncements"
                                 SET "ReportCount" = report_counts."Count"
                                 FROM (
                                     SELECT "AnnouncementId", COUNT(*)::integer AS "Count"
                                     FROM "AnnouncementReports"
                                     GROUP BY "AnnouncementId"
                                 ) AS report_counts
                                 WHERE "AnimalAnnouncements"."Id" = report_counts."AnnouncementId";
                                 """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReportCount",
                table: "AnimalAnnouncements");
        }
    }
}
