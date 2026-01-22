using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkHub.DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class AddInitial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Company",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CompanyName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CompanyWebUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompanyLogoUrl = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CompanyIndustry = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CompanySize = table.Column<int>(type: "int", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "varchar(15)", unicode: false, maxLength: 15, nullable: false),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Info = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Company__3214EC070F87BB5D", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Phone = table.Column<string>(type: "varchar(12)", unicode: false, maxLength: 12, nullable: true),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    IsVerified = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    Rating = table.Column<double>(type: "float", nullable: true),
                    FullName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvatarUrl = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Age = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__User__3214EC0794ADE483", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Employer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: true),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Employer__3214EC07E3EE2679", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Employer_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Employer_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Seeker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Bio = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CvUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    EducationLevel = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Major = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Schedule = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Seeker__3214EC072FF42672", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Seeker_User",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RecruitmentInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployerId = table.Column<int>(type: "int", nullable: false),
                    CompanyId = table.Column<int>(type: "int", nullable: false),
                    JobName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    JobType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Location = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Salary = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())"),
                    Status = table.Column<bool>(type: "bit", nullable: false, defaultValue: true),
                    Schedule = table.Column<string>(type: "varchar(200)", unicode: false, maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Recruitm__3214EC0755D60B5E", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recruitment_Company",
                        column: x => x.CompanyId,
                        principalTable: "Company",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Recruitment_Employer",
                        column: x => x.EmployerId,
                        principalTable: "Employer",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Application",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SeekerId = table.Column<int>(type: "int", nullable: false),
                    RecruitmentId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "(sysdatetime())")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Applicat__3214EC077AFC05A6", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Application_Recruitment",
                        column: x => x.RecruitmentId,
                        principalTable: "RecruitmentInfo",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Application_Seeker",
                        column: x => x.SeekerId,
                        principalTable: "Seeker",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Application_RecruitmentId",
                table: "Application",
                column: "RecruitmentId");

            migrationBuilder.CreateIndex(
                name: "UQ_Application",
                table: "Application",
                columns: new[] { "SeekerId", "RecruitmentId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Employer_CompanyId",
                table: "Employer",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_Employer_UserId",
                table: "Employer",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentInfo_CompanyId",
                table: "RecruitmentInfo",
                column: "CompanyId");

            migrationBuilder.CreateIndex(
                name: "IX_RecruitmentInfo_EmployerId",
                table: "RecruitmentInfo",
                column: "EmployerId");

            migrationBuilder.CreateIndex(
                name: "IX_Seeker_UserId",
                table: "Seeker",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "UQ__User__A9D1053491BE2FC7",
                table: "User",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Application");

            migrationBuilder.DropTable(
                name: "RecruitmentInfo");

            migrationBuilder.DropTable(
                name: "Seeker");

            migrationBuilder.DropTable(
                name: "Employer");

            migrationBuilder.DropTable(
                name: "Company");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
