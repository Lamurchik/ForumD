using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Forum.Migrations
{
    /// <inheritdoc />
    public partial class SocialNotificationChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CommentId",
                table: "SocialNotifications",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "SocialNotifications",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "NotificationType",
                table: "SocialNotifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "SocialNotifications",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_SocialNotifications_CommentId",
                table: "SocialNotifications",
                column: "CommentId");

            migrationBuilder.CreateIndex(
                name: "IX_SocialNotifications_UserId",
                table: "SocialNotifications",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_SocialNotifications_Comments_CommentId",
                table: "SocialNotifications",
                column: "CommentId",
                principalTable: "Comments",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SocialNotifications_Users_UserId",
                table: "SocialNotifications",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SocialNotifications_Comments_CommentId",
                table: "SocialNotifications");

            migrationBuilder.DropForeignKey(
                name: "FK_SocialNotifications_Users_UserId",
                table: "SocialNotifications");

            migrationBuilder.DropIndex(
                name: "IX_SocialNotifications_CommentId",
                table: "SocialNotifications");

            migrationBuilder.DropIndex(
                name: "IX_SocialNotifications_UserId",
                table: "SocialNotifications");

            migrationBuilder.DropColumn(
                name: "CommentId",
                table: "SocialNotifications");

            migrationBuilder.DropColumn(
                name: "IsRead",
                table: "SocialNotifications");

            migrationBuilder.DropColumn(
                name: "NotificationType",
                table: "SocialNotifications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "SocialNotifications");
        }
    }
}
