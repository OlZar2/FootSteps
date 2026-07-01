using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FS.Migrations.Migrations
{
    /// <inheritdoc />
    public partial class SearchErrorMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ErrorCode",
                table: "SearchRequests",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ErrorCode",
                table: "SearchRequests");
        }
    }
}
