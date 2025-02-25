using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportPortal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class INIT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Projects",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    ProjectStatus = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserRole = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Runs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    ProjectId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Runs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalSchema: "dbo",
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Folders",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ParentId = table.Column<int>(type: "int", nullable: true),
                    FolderLevel = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    RunId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Folders", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Folders_Folders_ParentId",
                        column: x => x.ParentId,
                        principalSchema: "dbo",
                        principalTable: "Folders",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Folders_Runs_RunId",
                        column: x => x.RunId,
                        principalSchema: "dbo",
                        principalTable: "Runs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RunId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    FolderId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_Folders_FolderId",
                        column: x => x.FolderId,
                        principalSchema: "dbo",
                        principalTable: "Folders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestResults",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    ErrorMessage = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    StackTrace = table.Column<string>(type: "nvarchar(MAX)", nullable: false),
                    ScreenShot = table.Column<byte[]>(type: "varbinary(max)", nullable: false),
                    TestOutcome = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestResults_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "dbo",
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestReviews",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    ReviewerId = table.Column<int>(type: "int", nullable: true),
                    Comments = table.Column<string>(type: "nvarchar(MAX)", nullable: true),
                    TestReviewOutcome = table.Column<int>(type: "int", nullable: false),
                    ProductBug = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestReviews_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "dbo",
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestReviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Folders_ParentId",
                schema: "dbo",
                table: "Folders",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_RunId",
                schema: "dbo",
                table: "Folders",
                column: "RunId");

            migrationBuilder.CreateIndex(
                name: "IX_Runs_ProjectId",
                schema: "dbo",
                table: "Runs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_TestId",
                schema: "dbo",
                table: "TestResults",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestReviews_ReviewerId",
                schema: "dbo",
                table: "TestReviews",
                column: "ReviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_TestReviews_TestId",
                schema: "dbo",
                table: "TestReviews",
                column: "TestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tests_FolderId",
                schema: "dbo",
                table: "Tests",
                column: "FolderId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestResults",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "TestReviews",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Tests",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Folders",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Runs",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Projects",
                schema: "dbo");
        }
    }
}
