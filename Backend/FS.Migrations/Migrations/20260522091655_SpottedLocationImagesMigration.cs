using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SpottedLocationImagesMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SpottedLocationId",
                table: "Images",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Images_SpottedLocationId",
                table: "Images",
                column: "SpottedLocationId");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_SpottedLocations_SpottedLocationId",
                table: "Images",
                column: "SpottedLocationId",
                principalTable: "SpottedLocations",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_SpottedLocations_SpottedLocationId",
                table: "Images");

            migrationBuilder.DropIndex(
                name: "IX_Images_SpottedLocationId",
                table: "Images");

            migrationBuilder.DropColumn(
                name: "SpottedLocationId",
                table: "Images");
        }
    }
}
