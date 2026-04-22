using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShopOnline.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class _20260420155142 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_EntityRefreshToken",
                table: "EntityRefreshToken");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Brand",
                table: "Brand");

            migrationBuilder.RenameTable(
                name: "EntityRefreshToken",
                newName: "Auth_RefreshToken");

            migrationBuilder.RenameTable(
                name: "Brand",
                newName: "MatterData_Brand");

            migrationBuilder.RenameIndex(
                name: "IX_EntityRefreshToken_Id",
                table: "Auth_RefreshToken",
                newName: "IX_Auth_RefreshToken_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Brand_Id",
                table: "MatterData_Brand",
                newName: "IX_MatterData_Brand_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Auth_RefreshToken",
                table: "Auth_RefreshToken",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MatterData_Brand",
                table: "MatterData_Brand",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_MatterData_Brand",
                table: "MatterData_Brand");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Auth_RefreshToken",
                table: "Auth_RefreshToken");

            migrationBuilder.RenameTable(
                name: "MatterData_Brand",
                newName: "Brand");

            migrationBuilder.RenameTable(
                name: "Auth_RefreshToken",
                newName: "EntityRefreshToken");

            migrationBuilder.RenameIndex(
                name: "IX_MatterData_Brand_Id",
                table: "Brand",
                newName: "IX_Brand_Id");

            migrationBuilder.RenameIndex(
                name: "IX_Auth_RefreshToken_Id",
                table: "EntityRefreshToken",
                newName: "IX_EntityRefreshToken_Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Brand",
                table: "Brand",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EntityRefreshToken",
                table: "EntityRefreshToken",
                column: "Id");
        }
    }
}
