using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportPortal.DAL.Migrations
{
    /// <inheritdoc />
    public partial class AddRunDbSet : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Run_Projects_ProjectId",
                table: "Run");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Run",
                table: "Run");

            migrationBuilder.DropIndex(
                name: "IX_Run_ProjectId",
                table: "Run");

            migrationBuilder.DropColumn(
                name: "ChildrenIds",
                table: "Tests");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "Folders");

            migrationBuilder.DropColumn(
                name: "ChildrenIds",
                table: "Run");

            migrationBuilder.DropColumn(
                name: "ParentId",
                table: "Run");

            migrationBuilder.RenameTable(
                name: "Run",
                newName: "Runs");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Tests",
                newName: "RunId");

            migrationBuilder.RenameColumn(
                name: "ParentId",
                table: "Tests",
                newName: "FolderId");

            migrationBuilder.RenameColumn(
                name: "ChildrenIds",
                table: "Folders",
                newName: "TestIds");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "Runs",
                newName: "RootFolderId");

            migrationBuilder.AddColumn<int>(
                name: "ProjectStatus",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RunIds",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "Folders",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ChildFolderIds",
                table: "Folders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Runs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Runs",
                table: "Runs",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Runs",
                table: "Runs");

            migrationBuilder.DropColumn(
                name: "ProjectStatus",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "RunIds",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ChildFolderIds",
                table: "Folders");

            migrationBuilder.RenameTable(
                name: "Runs",
                newName: "Run");

            migrationBuilder.RenameColumn(
                name: "RunId",
                table: "Tests",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "FolderId",
                table: "Tests",
                newName: "ParentId");

            migrationBuilder.RenameColumn(
                name: "TestIds",
                table: "Folders",
                newName: "ChildrenIds");

            migrationBuilder.RenameColumn(
                name: "RootFolderId",
                table: "Run",
                newName: "Type");

            migrationBuilder.AddColumn<string>(
                name: "ChildrenIds",
                table: "Tests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<int>(
                name: "ParentId",
                table: "Folders",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "Folders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Run",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "ChildrenIds",
                table: "Run",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ParentId",
                table: "Run",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Run",
                table: "Run",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Run_ProjectId",
                table: "Run",
                column: "ProjectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Run_Projects_ProjectId",
                table: "Run",
                column: "ProjectId",
                principalTable: "Projects",
                principalColumn: "Id");
        }
    }
}
