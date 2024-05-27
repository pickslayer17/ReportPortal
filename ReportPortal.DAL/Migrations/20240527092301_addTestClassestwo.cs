using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportPortal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class addTestClassestwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestResultId",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "TestRunItemId",
                table: "TestResults",
                newName: "TestId");

            migrationBuilder.AddColumn<string>(
                name: "TestResultIds",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestResultIds",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "TestId",
                table: "TestResults",
                newName: "TestRunItemId");

            migrationBuilder.AddColumn<int>(
                name: "TestResultId",
                table: "Tests",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
