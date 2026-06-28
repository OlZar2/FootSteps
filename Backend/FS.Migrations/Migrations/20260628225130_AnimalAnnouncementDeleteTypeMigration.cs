using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class AnimalAnnouncementDeleteTypeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DeleteType",
                table: "AnimalAnnouncements",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql("""
                                 UPDATE "AnimalAnnouncements"
                                 SET "DeleteType" = 1
                                 WHERE "IsDeleted" = TRUE
                                 """);

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AnimalAnnouncements");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AnimalAnnouncements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.Sql("""
                                 UPDATE "AnimalAnnouncements"
                                 SET "IsDeleted" = TRUE
                                 WHERE "DeleteType" IS NOT NULL
                                 """);

            migrationBuilder.DropColumn(
                name: "DeleteType",
                table: "AnimalAnnouncements");
        }
    }
}
