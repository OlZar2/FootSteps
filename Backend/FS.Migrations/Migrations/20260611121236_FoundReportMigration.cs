using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class FoundReportMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "FoundReportId",
                table: "Images",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FoundReports",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    FoundUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    MissingAnnouncementId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FoundReports", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FoundReports_AnimalAnnouncements_MissingAnnouncementId",
                        column: x => x.MissingAnnouncementId,
                        principalTable: "AnimalAnnouncements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FoundReports_Users_FoundUserId",
                        column: x => x.FoundUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_FoundReportId",
                table: "Images",
                column: "FoundReportId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundReports_FoundUserId",
                table: "FoundReports",
                column: "FoundUserId");

            migrationBuilder.CreateIndex(
                name: "IX_FoundReports_MissingAnnouncementId",
                table: "FoundReports",
                column: "MissingAnnouncementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_FoundReports_FoundReportId",
                table: "Images",
                column: "FoundReportId",
                principalTable: "FoundReports",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_FoundReports_FoundReportId",
                table: "Images");

            migrationBuilder.DropTable(
                name: "FoundReports");

            migrationBuilder.DropIndex(
                name: "IX_Images_FoundReportId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "FoundReportId",
                table: "Images");
        }
    }
}
