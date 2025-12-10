using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class NullableDistrictMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FullPlace",
                table: "AnimalAnnouncements");

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "AnimalAnnouncements",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "House",
                table: "AnimalAnnouncements",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Street",
                table: "AnimalAnnouncements",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "House",
                table: "AnimalAnnouncements");

            migrationBuilder.DropColumn(
                name: "Street",
                table: "AnimalAnnouncements");

            migrationBuilder.AlterColumn<string>(
                name: "District",
                table: "AnimalAnnouncements",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FullPlace",
                table: "AnimalAnnouncements",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
