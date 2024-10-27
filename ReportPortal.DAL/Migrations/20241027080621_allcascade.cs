using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportPortal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class allcascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Folders_ParentId",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Runs_RunId",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Projects_ProjectId",
                schema: "dbo",
                table: "Runs");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestId",
                schema: "dbo",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestReviews_Tests_TestId",
                schema: "dbo",
                table: "TestReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_TestReviews_Users_ReviewerId",
                schema: "dbo",
                table: "TestReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Folders_FolderId",
                schema: "dbo",
                table: "Tests");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Folders_ParentId",
                schema: "dbo",
                table: "Folders",
                column: "ParentId",
                principalSchema: "dbo",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Runs_RunId",
                schema: "dbo",
                table: "Folders",
                column: "RunId",
                principalSchema: "dbo",
                principalTable: "Runs",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Projects_ProjectId",
                schema: "dbo",
                table: "Runs",
                column: "ProjectId",
                principalSchema: "dbo",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestId",
                schema: "dbo",
                table: "TestResults",
                column: "TestId",
                principalSchema: "dbo",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestReviews_Tests_TestId",
                schema: "dbo",
                table: "TestReviews",
                column: "TestId",
                principalSchema: "dbo",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestReviews_Users_ReviewerId",
                schema: "dbo",
                table: "TestReviews",
                column: "ReviewerId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Folders_FolderId",
                schema: "dbo",
                table: "Tests",
                column: "FolderId",
                principalSchema: "dbo",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Folders_ParentId",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Runs_RunId",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Projects_ProjectId",
                schema: "dbo",
                table: "Runs");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestId",
                schema: "dbo",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestReviews_Tests_TestId",
                schema: "dbo",
                table: "TestReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_TestReviews_Users_ReviewerId",
                schema: "dbo",
                table: "TestReviews");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Folders_FolderId",
                schema: "dbo",
                table: "Tests");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Folders_ParentId",
                schema: "dbo",
                table: "Folders",
                column: "ParentId",
                principalSchema: "dbo",
                principalTable: "Folders",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Runs_RunId",
                schema: "dbo",
                table: "Folders",
                column: "RunId",
                principalSchema: "dbo",
                principalTable: "Runs",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Projects_ProjectId",
                schema: "dbo",
                table: "Runs",
                column: "ProjectId",
                principalSchema: "dbo",
                principalTable: "Projects",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestId",
                schema: "dbo",
                table: "TestResults",
                column: "TestId",
                principalSchema: "dbo",
                principalTable: "Tests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestReviews_Tests_TestId",
                schema: "dbo",
                table: "TestReviews",
                column: "TestId",
                principalSchema: "dbo",
                principalTable: "Tests",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestReviews_Users_ReviewerId",
                schema: "dbo",
                table: "TestReviews",
                column: "ReviewerId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Folders_FolderId",
                schema: "dbo",
                table: "Tests",
                column: "FolderId",
                principalSchema: "dbo",
                principalTable: "Folders",
                principalColumn: "Id");
        }
    }
}
