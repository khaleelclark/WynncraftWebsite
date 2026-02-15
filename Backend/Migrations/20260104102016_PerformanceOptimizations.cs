using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class PerformanceOptimizations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "MinecraftUsername",
                table: "GuildMembers",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "GameName",
                table: "Games",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_CompletedDate_Uuid",
                table: "RaidsCompleted",
                columns: new[] { "CompletedDate", "Uuid" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_RaidId_CompletedDate",
                table: "RaidsCompleted",
                columns: new[] { "RaidId", "CompletedDate" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_RaidInstanceId",
                table: "RaidsCompleted",
                column: "RaidInstanceId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_Uuid",
                table: "RaidsCompleted",
                column: "Uuid"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHistoricalStats_GuildMemberId_SyncDate",
                table: "PlayerHistoricalStats",
                columns: new[] { "GuildMemberId", "SyncDate" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_LastSynced",
                table: "GuildMembers",
                column: "LastSynced"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_MinecraftUsername",
                table: "GuildMembers",
                column: "MinecraftUsername"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_Uuid",
                table: "GuildMembers",
                column: "Uuid",
                unique: true
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

            migrationBuilder.CreateIndex(
                name: "IX_Games_GameName",
                table: "Games",
                column: "GameName"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventStart",
                table: "Events",
                column: "EventStart"
            );

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventStart_EventEnd",
                table: "Events",
                columns: new[] { "EventStart", "EventEnd" }
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_CompletedDate_Uuid",
                table: "RaidsCompleted"
            );

            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_RaidId_CompletedDate",
                table: "RaidsCompleted"
            );

            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_RaidInstanceId",
                table: "RaidsCompleted"
            );

            migrationBuilder.DropIndex(name: "IX_RaidsCompleted_Uuid", table: "RaidsCompleted");

            migrationBuilder.DropIndex(
                name: "IX_PlayerHistoricalStats_GuildMemberId_SyncDate",
                table: "PlayerHistoricalStats"
            );

            migrationBuilder.DropIndex(name: "IX_GuildMembers_LastSynced", table: "GuildMembers");

            migrationBuilder.DropIndex(
                name: "IX_GuildMembers_MinecraftUsername",
                table: "GuildMembers"
            );

            migrationBuilder.DropIndex(name: "IX_GuildMembers_Uuid", table: "GuildMembers");

            migrationBuilder.DropIndex(
                name: "IX_GuildMemberMedals_MedalId",
                table: "GuildMemberMedals"
            );

            migrationBuilder.DropIndex(
                name: "IX_GuildMemberGames_GameId",
                table: "GuildMemberGames"
            );

            migrationBuilder.DropIndex(name: "IX_Games_GameName", table: "Games");

            migrationBuilder.DropIndex(name: "IX_Events_EventStart", table: "Events");

            migrationBuilder.DropIndex(name: "IX_Events_EventStart_EventEnd", table: "Events");

            migrationBuilder.AlterColumn<string>(
                name: "MinecraftUsername",
                table: "GuildMembers",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true
            );

            migrationBuilder.AlterColumn<string>(
                name: "GameName",
                table: "Games",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true
            );
        }
    }
}
