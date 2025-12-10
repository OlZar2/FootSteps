using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class NotificationDeliveryDeviceIdMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationDeliveries_Users_UserId",
                table: "NotificationDeliveries");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "NotificationDeliveries",
                newName: "UserDeviceId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationDeliveries_UserId",
                table: "NotificationDeliveries",
                newName: "IX_NotificationDeliveries_UserDeviceId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationDeliveries_UserDevices_UserDeviceId",
                table: "NotificationDeliveries",
                column: "UserDeviceId",
                principalTable: "UserDevices",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NotificationDeliveries_UserDevices_UserDeviceId",
                table: "NotificationDeliveries");

            migrationBuilder.RenameColumn(
                name: "UserDeviceId",
                table: "NotificationDeliveries",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_NotificationDeliveries_UserDeviceId",
                table: "NotificationDeliveries",
                newName: "IX_NotificationDeliveries_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_NotificationDeliveries_Users_UserId",
                table: "NotificationDeliveries",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
