using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddRecruitmentEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Category",
                table: "Recruitment");

            migrationBuilder.RenameColumn(
                name: "JobType",
                table: "Recruitment",
                newName: "JobTypeId");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "Recruitment",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Categories",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Categories", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "JobTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JobTypes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Recruitment_CategoryId",
                table: "Recruitment",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Recruitment_JobTypeId",
                table: "Recruitment",
                column: "JobTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Recruitment_Categories_CategoryId",
                table: "Recruitment",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Recruitment_JobTypes_JobTypeId",
                table: "Recruitment",
                column: "JobTypeId",
                principalTable: "JobTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Recruitment_Categories_CategoryId",
                table: "Recruitment");

            migrationBuilder.DropForeignKey(
                name: "FK_Recruitment_JobTypes_JobTypeId",
                table: "Recruitment");

            migrationBuilder.DropTable(
                name: "Categories");

            migrationBuilder.DropTable(
                name: "JobTypes");

            migrationBuilder.DropIndex(
                name: "IX_Recruitment_CategoryId",
                table: "Recruitment");

            migrationBuilder.DropIndex(
                name: "IX_Recruitment_JobTypeId",
                table: "Recruitment");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "Recruitment");

            migrationBuilder.RenameColumn(
                name: "JobTypeId",
                table: "Recruitment",
                newName: "JobType");

            migrationBuilder.AddColumn<string>(
                name: "Category",
                table: "Recruitment",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);
        }
    }
}
