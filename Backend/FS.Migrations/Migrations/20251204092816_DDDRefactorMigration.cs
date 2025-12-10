using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class DDDRefactorMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimilarAnnouncements_AnimalAnnouncements_SimilarMissingAnno~",
                table: "SimilarAnnouncements");

            migrationBuilder.DropForeignKey(
                name: "FK_SimilarAnnouncements_AnimalAnnouncements_SimilarStreetAnnou~",
                table: "SimilarAnnouncements");

            migrationBuilder.RenameColumn(
                name: "SimilarStreetAnnouncementsId",
                table: "SimilarAnnouncements",
                newName: "MissingAnnouncementId");

            migrationBuilder.RenameColumn(
                name: "SimilarMissingAnnouncementsId",
                table: "SimilarAnnouncements",
                newName: "StreetPetAnnouncementId");

            migrationBuilder.RenameIndex(
                name: "IX_SimilarAnnouncements_SimilarStreetAnnouncementsId",
                table: "SimilarAnnouncements",
                newName: "IX_SimilarAnnouncements_MissingAnnouncementId");

            migrationBuilder.RenameColumn(
                name: "Path",
                table: "Images",
                newName: "S3Key");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SimilarAnnouncements",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "BucketURL",
                table: "Images",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FullImagePath",
                table: "Images",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_SimilarAnnouncements_AnimalAnnouncements_MissingAnnouncemen~",
                table: "SimilarAnnouncements",
                column: "MissingAnnouncementId",
                principalTable: "AnimalAnnouncements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SimilarAnnouncements_AnimalAnnouncements_StreetPetAnnouncem~",
                table: "SimilarAnnouncements",
                column: "StreetPetAnnouncementId",
                principalTable: "AnimalAnnouncements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SimilarAnnouncements_AnimalAnnouncements_MissingAnnouncemen~",
                table: "SimilarAnnouncements");

            migrationBuilder.DropForeignKey(
                name: "FK_SimilarAnnouncements_AnimalAnnouncements_StreetPetAnnouncem~",
                table: "SimilarAnnouncements");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SimilarAnnouncements");

            migrationBuilder.DropColumn(
                name: "BucketURL",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "FullImagePath",
                table: "Images");

            migrationBuilder.RenameColumn(
                name: "MissingAnnouncementId",
                table: "SimilarAnnouncements",
                newName: "SimilarStreetAnnouncementsId");

            migrationBuilder.RenameColumn(
                name: "StreetPetAnnouncementId",
                table: "SimilarAnnouncements",
                newName: "SimilarMissingAnnouncementsId");

            migrationBuilder.RenameIndex(
                name: "IX_SimilarAnnouncements_MissingAnnouncementId",
                table: "SimilarAnnouncements",
                newName: "IX_SimilarAnnouncements_SimilarStreetAnnouncementsId");

            migrationBuilder.RenameColumn(
                name: "S3Key",
                table: "Images",
                newName: "Path");

            migrationBuilder.AddForeignKey(
                name: "FK_SimilarAnnouncements_AnimalAnnouncements_SimilarMissingAnno~",
                table: "SimilarAnnouncements",
                column: "SimilarMissingAnnouncementsId",
                principalTable: "AnimalAnnouncements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SimilarAnnouncements_AnimalAnnouncements_SimilarStreetAnnou~",
                table: "SimilarAnnouncements",
                column: "SimilarStreetAnnouncementsId",
                principalTable: "AnimalAnnouncements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
