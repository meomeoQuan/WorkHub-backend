using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Provider",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProviderId",
                table: "User",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Provider",
                table: "User");

            migrationBuilder.DropColumn(
                name: "ProviderId",
                table: "User");
        }
    }
}
