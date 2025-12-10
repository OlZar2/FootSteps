using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class isAnnounceCompletedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Announcements",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MissingAnnouncement_IsCompleted",
                table: "Announcements",
                type: "boolean",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "MissingAnnouncement_IsCompleted",
                table: "Announcements");
        }
    }
}
