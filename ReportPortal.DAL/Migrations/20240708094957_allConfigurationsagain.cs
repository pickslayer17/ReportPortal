using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportPortal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class allConfigurationsagain : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Runs_Projects_ProjectId",
                table: "Runs");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Folders_FolderId",
                table: "Tests");

            migrationBuilder.RenameTable(
                name: "Tests",
                newName: "Tests",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "TestResults",
                newName: "TestResults",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Runs",
                newName: "Runs",
                newSchema: "dbo");

            migrationBuilder.RenameTable(
                name: "Projects",
                newName: "Projects",
                newSchema: "dbo");

            migrationBuilder.AddColumn<int>(
                name: "RunId",
                schema: "dbo",
                table: "Folders",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "Tests",
                type: "nvarchar(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "StackTrace",
                schema: "dbo",
                table: "TestResults",
                type: "nvarchar(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                schema: "dbo",
                table: "TestResults",
                type: "nvarchar(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "Runs",
                type: "nvarchar(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "RootFolderId",
                schema: "dbo",
                table: "Runs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                schema: "dbo",
                table: "Projects",
                type: "nvarchar(MAX)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Folders_RunId",
                schema: "dbo",
                table: "Folders",
                column: "RunId",
                unique: true,
                filter: "[RunId] IS NOT NULL");

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
                name: "FK_Tests_Folders_FolderId",
                schema: "dbo",
                table: "Tests",
                column: "FolderId",
                principalSchema: "dbo",
                principalTable: "Folders",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "FK_Tests_Folders_FolderId",
                schema: "dbo",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Folders_RunId",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "RunId",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "RootFolderId",
                schema: "dbo",
                table: "Runs");

            migrationBuilder.RenameTable(
                name: "Tests",
                schema: "dbo",
                newName: "Tests");

            migrationBuilder.RenameTable(
                name: "TestResults",
                schema: "dbo",
                newName: "TestResults");

            migrationBuilder.RenameTable(
                name: "Runs",
                schema: "dbo",
                newName: "Runs");

            migrationBuilder.RenameTable(
                name: "Projects",
                schema: "dbo",
                newName: "Projects");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "StackTrace",
                table: "TestResults",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "ErrorMessage",
                table: "TestResults",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Runs",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(MAX)");

            migrationBuilder.AddForeignKey(
                name: "FK_Runs_Projects_ProjectId",
                table: "Runs",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_TestId",
                table: "TestResults",
                column: "TestId",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Folders_FolderId",
                table: "Tests",
                column: "FolderId",
                principalSchema: "dbo",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
