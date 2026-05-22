using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class LastLocationToGeographyMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "LastCoordinates",
                table: "Users",
                type: "geography(Point,4326)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry(Point,4326)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Point>(
                name: "LastCoordinates",
                table: "Users",
                type: "geometry(Point,4326)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geography(Point,4326)",
                oldNullable: true);
        }
    }
}
