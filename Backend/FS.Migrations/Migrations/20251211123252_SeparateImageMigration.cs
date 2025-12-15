using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Pgvector;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SeparateImageMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchRequests_SearchRequestImages_ImageId",
                table: "SearchRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_AnimalAnnouncementImages_AvatarImageId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "AnimalAnnouncementImages");

            migrationBuilder.DropTable(
                name: "SearchRequestImages");

            migrationBuilder.CreateTable(
                name: "Images",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    S3Key = table.Column<string>(type: "text", nullable: false),
                    BucketURL = table.Column<string>(type: "text", nullable: false),
                    FullImagePath = table.Column<string>(type: "text", nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(512)", nullable: true),
                    AnimalAnnouncementId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Images", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Images_AnimalAnnouncements_AnimalAnnouncementId",
                        column: x => x.AnimalAnnouncementId,
                        principalTable: "AnimalAnnouncements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Images_AnimalAnnouncementId",
                table: "Images",
                column: "AnimalAnnouncementId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchRequests_Images_ImageId",
                table: "SearchRequests",
                column: "ImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Images_AvatarImageId",
                table: "Users",
                column: "AvatarImageId",
                principalTable: "Images",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SearchRequests_Images_ImageId",
                table: "SearchRequests");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Images_AvatarImageId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Images");

            migrationBuilder.CreateTable(
                name: "AnimalAnnouncementImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalAnnouncementId = table.Column<Guid>(type: "uuid", nullable: true),
                    BucketURL = table.Column<string>(type: "text", nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(512)", nullable: true),
                    FullImagePath = table.Column<string>(type: "text", nullable: false),
                    S3Key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalAnnouncementImages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalAnnouncementImages_AnimalAnnouncements_AnimalAnnounce~",
                        column: x => x.AnimalAnnouncementId,
                        principalTable: "AnimalAnnouncements",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SearchRequestImages",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BucketURL = table.Column<string>(type: "text", nullable: false),
                    Embedding = table.Column<Vector>(type: "vector(512)", nullable: true),
                    FullImagePath = table.Column<string>(type: "text", nullable: false),
                    S3Key = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SearchRequestImages", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AnimalAnnouncementImages_AnimalAnnouncementId",
                table: "AnimalAnnouncementImages",
                column: "AnimalAnnouncementId");

            migrationBuilder.AddForeignKey(
                name: "FK_SearchRequests_SearchRequestImages_ImageId",
                table: "SearchRequests",
                column: "ImageId",
                principalTable: "SearchRequestImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_AnimalAnnouncementImages_AvatarImageId",
                table: "Users",
                column: "AvatarImageId",
                principalTable: "AnimalAnnouncementImages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
