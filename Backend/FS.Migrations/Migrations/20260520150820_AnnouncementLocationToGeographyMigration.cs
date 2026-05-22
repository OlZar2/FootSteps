using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AnnouncementLocationToGeographyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "AnimalAnnouncements",
                type: "geography(Point,4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry(Point,4326)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "AnimalAnnouncements",
                type: "geometry(Point,4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geography(Point,4326)");
        }
    }
}
