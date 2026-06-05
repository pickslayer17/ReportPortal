using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportPortal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddSubprojects : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Projects_ProjectId",
                schema: "dbo",
                table: "Runs");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                schema: "dbo",
                table: "Runs",
                newName: "SubprojectId");

            migrationBuilder.RenameIndex(
                name: "IX_Runs_ProjectId",
                schema: "dbo",
                table: "Runs",
                newName: "IX_Runs_SubprojectId");

            migrationBuilder.CreateTable(
                name: "Subprojects",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(256)", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subprojects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subprojects_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "dbo",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserSubprojects",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    SubprojectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSubprojects", x => new { x.UserId, x.SubprojectId });
                    table.ForeignKey(
                        name: "FK_UserSubprojects_Subprojects_SubprojectId",
                        column: x => x.SubprojectId,
                        principalSchema: "dbo",
                        principalTable: "Subprojects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserSubprojects_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Subprojects_ProjectId",
                schema: "dbo",
                table: "Subprojects",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_UserSubprojects_SubprojectId",
                table: "UserSubprojects",
                column: "SubprojectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Subprojects_SubprojectId",
                schema: "dbo",
                table: "Runs",
                column: "SubprojectId",
                principalSchema: "dbo",
                principalTable: "Subprojects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Subprojects_SubprojectId",
                schema: "dbo",
                table: "Runs");

            migrationBuilder.DropTable(
                name: "UserSubprojects");

            migrationBuilder.DropTable(
                name: "Subprojects",
                schema: "dbo");

            migrationBuilder.RenameColumn(
                name: "SubprojectId",
                schema: "dbo",
                table: "Runs",
                newName: "ProjectId");

            migrationBuilder.RenameIndex(
                name: "IX_Runs_SubprojectId",
                schema: "dbo",
                table: "Runs",
                newName: "IX_Runs_ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Projects_ProjectId",
                schema: "dbo",
                table: "Runs",
                column: "ProjectId",
                principalSchema: "dbo",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
