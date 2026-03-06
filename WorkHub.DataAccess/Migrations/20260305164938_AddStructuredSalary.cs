using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddStructuredSalary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "MaxSalary",
                table: "Recruitment",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "MinSalary",
                table: "Recruitment",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SalaryCurrency",
                table: "Recruitment",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SalaryCycle",
                table: "Recruitment",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxSalary",
                table: "Recruitment");

            migrationBuilder.DropColumn(
                name: "MinSalary",
                table: "Recruitment");

            migrationBuilder.DropColumn(
                name: "SalaryCurrency",
                table: "Recruitment");

            migrationBuilder.DropColumn(
                name: "SalaryCycle",
                table: "Recruitment");
        }
    }
}
