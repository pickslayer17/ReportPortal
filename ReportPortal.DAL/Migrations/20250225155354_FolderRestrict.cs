using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportPortal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class FolderRestrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Folders_ParentId",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Folders_ParentId",
                schema: "dbo",
                table: "Folders",
                column: "ParentId",
                principalSchema: "dbo",
                principalTable: "Folders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Folders_Folders_ParentId",
                schema: "dbo",
                table: "Folders");

            migrationBuilder.AddForeignKey(
                name: "FK_Folders_Folders_ParentId",
                schema: "dbo",
                table: "Folders",
                column: "ParentId",
                principalSchema: "dbo",
                principalTable: "Folders",
                principalColumn: "Id");
        }
    }
}
