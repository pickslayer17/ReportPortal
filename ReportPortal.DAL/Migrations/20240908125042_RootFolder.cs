using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportPortal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class RootFolder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folders_RunId",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "RootFolderId",
                schema: "dbo",
                table: "Runs");

            migrationBuilder.AddColumn<int>(
                name: "FolderLevel",
                schema: "dbo",
                table: "Folders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Folders_RunId",
                schema: "dbo",
                table: "Folders",
                column: "RunId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Folders_RunId",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "FolderLevel",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.AddColumn<int>(
                name: "RootFolderId",
                schema: "dbo",
                table: "Runs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Folders_RunId",
                schema: "dbo",
                table: "Folders",
                column: "RunId",
                unique: true,
                filter: "[RunId] IS NOT NULL");
        }
    }
}
