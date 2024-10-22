using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportPortal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class TestReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                    TestReviewOutcome = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestReviews", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestReviews_Tests_TestId",
                        column: x => x.TestId,
                        principalSchema: "dbo",
                        principalTable: "Tests",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestReviews_Users_ReviewerId",
                        column: x => x.ReviewerId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestReviews",
                schema: "dbo");
        }
    }
}
