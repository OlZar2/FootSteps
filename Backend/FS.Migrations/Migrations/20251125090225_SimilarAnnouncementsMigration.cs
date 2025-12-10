using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SimilarAnnouncementsMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SimilarAnnouncements",
                columns: table => new
                {
                    SimilarMissingAnnouncementsId = table.Column<Guid>(type: "uuid", nullable: false),
                    SimilarStreetAnnouncementsId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SimilarAnnouncements", x => new { x.SimilarMissingAnnouncementsId, x.SimilarStreetAnnouncementsId });
                    table.ForeignKey(
                        name: "FK_SimilarAnnouncements_AnimalAnnouncements_SimilarMissingAnno~",
                        column: x => x.SimilarMissingAnnouncementsId,
                        principalTable: "AnimalAnnouncements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SimilarAnnouncements_AnimalAnnouncements_SimilarStreetAnnou~",
                        column: x => x.SimilarStreetAnnouncementsId,
                        principalTable: "AnimalAnnouncements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SimilarAnnouncements_SimilarStreetAnnouncementsId",
                table: "SimilarAnnouncements",
                column: "SimilarStreetAnnouncementsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SimilarAnnouncements");
        }
    }
}
