using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class addNonNullableForeignkey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_User_ApplicationUserId",
                schema: "Identity",
                table: "Contact");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                schema: "Identity",
                table: "Contact",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_User_ApplicationUserId",
                schema: "Identity",
                table: "Contact",
                column: "ApplicationUserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_User_ApplicationUserId",
                schema: "Identity",
                table: "Contact");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                schema: "Identity",
                table: "Contact",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_User_ApplicationUserId",
                schema: "Identity",
                table: "Contact",
                column: "ApplicationUserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }
    }
}
