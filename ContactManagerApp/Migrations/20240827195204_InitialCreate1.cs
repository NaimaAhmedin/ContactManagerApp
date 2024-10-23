using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ContactManagerApp.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_User_UserId",
                schema: "Identity",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Contact_UserId",
                schema: "Identity",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "Identity",
                table: "Contact");

            migrationBuilder.AddColumn<string>(
                name: "ApplicationUserId",
                schema: "Identity",
                table: "Contact",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Contact_ApplicationUserId",
                schema: "Identity",
                table: "Contact",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_User_ApplicationUserId",
                schema: "Identity",
                table: "Contact",
                column: "ApplicationUserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Contact_User_ApplicationUserId",
                schema: "Identity",
                table: "Contact");

            migrationBuilder.DropIndex(
                name: "IX_Contact_ApplicationUserId",
                schema: "Identity",
                table: "Contact");

            migrationBuilder.DropColumn(
                name: "ApplicationUserId",
                schema: "Identity",
                table: "Contact");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                schema: "Identity",
                table: "Contact",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Contact_UserId",
                schema: "Identity",
                table: "Contact",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Contact_User_UserId",
                schema: "Identity",
                table: "Contact",
                column: "UserId",
                principalSchema: "Identity",
                principalTable: "User",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
