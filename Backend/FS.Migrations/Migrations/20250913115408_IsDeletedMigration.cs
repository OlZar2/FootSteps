using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class IsDeletedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalAnnouncements_Users_CreatorId",
                table: "AnimalAnnouncements");

            migrationBuilder.AddColumn<int>(
                name: "DeleteReason",
                table: "AnimalAnnouncements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsDeleted",
                table: "AnimalAnnouncements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalAnnouncements_Users_CreatorId",
                table: "AnimalAnnouncements",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalAnnouncements_Users_CreatorId",
                table: "AnimalAnnouncements");

            migrationBuilder.DropColumn(
                name: "DeleteReason",
                table: "AnimalAnnouncements");

            migrationBuilder.DropColumn(
                name: "IsDeleted",
                table: "AnimalAnnouncements");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalAnnouncements_Users_CreatorId",
                table: "AnimalAnnouncements",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
