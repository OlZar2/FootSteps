using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class RemovePetTypeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Announcements_PetTypes_PetTypeId",
                table: "Announcements");

            migrationBuilder.DropTable(
                name: "PetTypes");

            migrationBuilder.DropIndex(
                name: "IX_Announcements_PetTypeId",
                table: "Announcements");

            migrationBuilder.DropColumn(
                name: "PetTypeId",
                table: "Announcements");

            migrationBuilder.AddColumn<int>(
                name: "PetType",
                table: "Announcements",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PetType",
                table: "Announcements");

            migrationBuilder.AddColumn<Guid>(
                name: "PetTypeId",
                table: "Announcements",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "PetTypes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Announcements_PetTypeId",
                table: "Announcements",
                column: "PetTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Announcements_PetTypes_PetTypeId",
                table: "Announcements",
                column: "PetTypeId",
                principalTable: "PetTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
