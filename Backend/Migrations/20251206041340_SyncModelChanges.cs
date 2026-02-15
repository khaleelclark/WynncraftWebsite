using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class SyncModelChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GuildMemberMedals_MedalId",
                table: "GuildMemberMedals"
            );

            migrationBuilder.DropIndex(
                name: "IX_GuildMemberGames_GameId",
                table: "GuildMemberGames"
            );

            migrationBuilder.DropColumn(name: "PlayerSkin", table: "GuildMembers");

            migrationBuilder.CreateIndex(
                name: "IX_GuildMemberMedals_MedalId_GuildMemberId",
                table: "GuildMemberMedals",
                columns: new[] { "MedalId", "GuildMemberId" },
                unique: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMemberGames_GameId_GuildMemberId",
                table: "GuildMemberGames",
                columns: new[] { "GameId", "GuildMemberId" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GuildMemberMedals_MedalId_GuildMemberId",
                table: "GuildMemberMedals"
            );

            migrationBuilder.DropIndex(
                name: "IX_GuildMemberGames_GameId_GuildMemberId",
                table: "GuildMemberGames"
            );

            migrationBuilder.AddColumn<string>(
                name: "PlayerSkin",
                table: "GuildMembers",
                type: "nvarchar(max)",
                nullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMemberMedals_MedalId",
                table: "GuildMemberMedals",
                column: "MedalId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMemberGames_GameId",
                table: "GuildMemberGames",
                column: "GameId"
            );
        }
    }
}
