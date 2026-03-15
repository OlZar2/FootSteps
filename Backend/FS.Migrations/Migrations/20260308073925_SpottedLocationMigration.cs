using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SpottedLocationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SpottedLocations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Location = table.Column<Point>(type: "geometry(Point,4326)", nullable: false),
                    SpottedUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    MissingAnnouncementId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpottedLocations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SpottedLocations_AnimalAnnouncements_MissingAnnouncementId",
                        column: x => x.MissingAnnouncementId,
                        principalTable: "AnimalAnnouncements",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SpottedLocations_Users_SpottedUserId",
                        column: x => x.SpottedUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SpottedLocations_MissingAnnouncementId",
                table: "SpottedLocations",
                column: "MissingAnnouncementId");

            migrationBuilder.CreateIndex(
                name: "IX_SpottedLocations_SpottedUserId",
                table: "SpottedLocations",
                column: "SpottedUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SpottedLocations");
        }
    }
}
