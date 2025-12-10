using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RebuildDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_Users_CreatorId",
                table: "Announcements");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_Announcements_AnnouncementId",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Announcements",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "MissingAnnouncement_Gender",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "MissingAnnouncement_IsCompleted",
                table: "Announcements");

            migrationBuilder.RenameTable(
                name: "Announcements",
                newName: "AnimalAnnouncements");

            migrationBuilder.RenameColumn(
                name: "AnnouncementId",
                table: "Images",
                newName: "AnimalAnnouncementId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_AnnouncementId",
                table: "Images",
                newName: "IX_Images_AnimalAnnouncementId");

            migrationBuilder.RenameColumn(
                name: "Place",
                table: "AnimalAnnouncements",
                newName: "FullPlace");

            migrationBuilder.RenameIndex(
                name: "IX_Announcements_CreatorId",
                table: "AnimalAnnouncements",
                newName: "IX_AnimalAnnouncements_CreatorId");

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "AnimalAnnouncements",
                type: "geometry(Point,4326)",
                nullable: false,
                oldClrType: typeof(Point),
                oldType: "geometry(Point,4326)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "AnimalAnnouncements",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Breed",
                table: "AnimalAnnouncements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Color",
                table: "AnimalAnnouncements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "District",
                table: "AnimalAnnouncements",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AnimalAnnouncements",
                table: "AnimalAnnouncements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AnimalAnnouncements_Users_CreatorId",
                table: "AnimalAnnouncements",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_AnimalAnnouncements_AnimalAnnouncementId",
                table: "Images",
                column: "AnimalAnnouncementId",
                principalTable: "AnimalAnnouncements",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AnimalAnnouncements_Users_CreatorId",
                table: "AnimalAnnouncements");

            migrationBuilder.DropForeignKey(
                name: "FK_Images_AnimalAnnouncements_AnimalAnnouncementId",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AnimalAnnouncements",
                table: "AnimalAnnouncements");

            migrationBuilder.DropColumn(
                name: "Breed",
                table: "AnimalAnnouncements");

            migrationBuilder.DropColumn(
                name: "Color",
                table: "AnimalAnnouncements");

            migrationBuilder.DropColumn(
                name: "District",
                table: "AnimalAnnouncements");

            migrationBuilder.RenameTable(
                name: "AnimalAnnouncements",
                newName: "Announcements");

            migrationBuilder.RenameColumn(
                name: "AnimalAnnouncementId",
                table: "Images",
                newName: "AnnouncementId");

            migrationBuilder.RenameIndex(
                name: "IX_Images_AnimalAnnouncementId",
                table: "Images",
                newName: "IX_Images_AnnouncementId");

            migrationBuilder.RenameColumn(
                name: "FullPlace",
                table: "Announcements",
                newName: "Place");

            migrationBuilder.RenameIndex(
                name: "IX_AnimalAnnouncements_CreatorId",
                table: "Announcements",
                newName: "IX_Announcements_CreatorId");

            migrationBuilder.AlterColumn<Point>(
                name: "Location",
                table: "Announcements",
                type: "geometry(Point,4326)",
                nullable: true,
                oldClrType: typeof(Point),
                oldType: "geometry(Point,4326)");

            migrationBuilder.AlterColumn<int>(
                name: "Gender",
                table: "Announcements",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int>(
                name: "MissingAnnouncement_Gender",
                table: "Announcements",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "MissingAnnouncement_IsCompleted",
                table: "Announcements",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Announcements",
                table: "Announcements",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_Users_CreatorId",
                table: "Announcements",
                column: "CreatorId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Announcements_AnnouncementId",
                table: "Images",
                column: "AnnouncementId",
                principalTable: "Announcements",
                principalColumn: "Id");
        }
    }
}
