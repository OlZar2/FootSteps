using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class EmailConfirmationMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EmailConfirmationToken",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsEmailConfirmed",
                table: "Users",
                type: "boolean",
                nullable: true);

            migrationBuilder.Sql("""
                                 UPDATE "Users"
                                 SET "IsEmailConfirmed" = TRUE
                                 WHERE "IsEmailConfirmed" IS NULL
                                 """);

            migrationBuilder.AlterColumn<bool>(
                name: "IsEmailConfirmed",
                table: "Users",
                type: "boolean",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "boolean",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RecipientEmail",
                table: "NotificationDeliveries",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmailConfirmationToken",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "IsEmailConfirmed",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RecipientEmail",
                table: "NotificationDeliveries");
        }
    }
}
