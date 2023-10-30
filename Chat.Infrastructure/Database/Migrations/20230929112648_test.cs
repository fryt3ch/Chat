using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chat.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ChatMessages_LastMessageId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats",
                column: "LastMessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ChatMessages_LastMessageId",
                table: "Chats",
                column: "LastMessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_ChatMessages_LastMessageId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_LastMessageId",
                table: "Chats",
                column: "LastMessageId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_ChatMessages_LastMessageId",
                table: "Chats",
                column: "LastMessageId",
                principalTable: "ChatMessages",
                principalColumn: "Id");
        }
    }
}
