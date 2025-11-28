using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class UserLocationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastLatitude",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastLongitude",
                table: "Users");

            migrationBuilder.AddColumn<Point>(
                name: "LastCoordinates",
                table: "Users",
                type: "geometry(Point,4326)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastCoordinates",
                table: "Users");

            migrationBuilder.AddColumn<double>(
                name: "LastLatitude",
                table: "Users",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "LastLongitude",
                table: "Users",
                type: "double precision",
                nullable: true);
        }
    }
}
