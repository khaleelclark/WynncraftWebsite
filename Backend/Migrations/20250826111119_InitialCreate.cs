using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    EventId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EventName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    EventType = table.Column<int>(type: "int", nullable: false),
                    EventStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EventEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Events", x => x.EventId);
                }
            );

            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    GameId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.GameId);
                }
            );

            migrationBuilder.CreateTable(
                name: "Medals",
                columns: table => new
                {
                    MedalId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedalName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medals", x => x.MedalId);
                }
            );

            migrationBuilder.CreateTable(
                name: "Raids",
                columns: table => new
                {
                    RaidId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaidName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SeasonRaiting = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Raids", x => x.RaidId);
                }
            );

            migrationBuilder.CreateTable(
                name: "Ranks",
                columns: table => new
                {
                    RankId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ranks", x => x.RankId);
                }
            );

            migrationBuilder.CreateTable(
                name: "GuildMembers",
                columns: table => new
                {
                    GuildMemberId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DiscordTag = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MinecraftUsername = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RankId = table.Column<int>(type: "int", nullable: false),
                    PlayerSkin = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    JoinDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WynncraftRank = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    HoursPlayed = table.Column<int>(type: "int", nullable: false),
                    WarsCompleted = table.Column<int>(type: "int", nullable: false),
                    WeekliesCompleted = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMembers", x => x.GuildMemberId);
                    table.ForeignKey(
                        name: "FK_GuildMembers_Ranks_RankId",
                        column: x => x.RankId,
                        principalTable: "Ranks",
                        principalColumn: "RankId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "GuildMemberGames",
                columns: table => new
                {
                    GuildMemberGameId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameId = table.Column<int>(type: "int", nullable: false),
                    GuildMemberId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMemberGames", x => x.GuildMemberGameId);
                    table.ForeignKey(
                        name: "FK_GuildMemberGames_Games_GameId",
                        column: x => x.GameId,
                        principalTable: "Games",
                        principalColumn: "GameId",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_GuildMemberGames_GuildMembers_GuildMemberId",
                        column: x => x.GuildMemberId,
                        principalTable: "GuildMembers",
                        principalColumn: "GuildMemberId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "GuildMemberMedals",
                columns: table => new
                {
                    GuildMemberMedalId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedalId = table.Column<int>(type: "int", nullable: false),
                    GuildMemberId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GuildMemberMedals", x => x.GuildMemberMedalId);
                    table.ForeignKey(
                        name: "FK_GuildMemberMedals_GuildMembers_GuildMemberId",
                        column: x => x.GuildMemberId,
                        principalTable: "GuildMembers",
                        principalColumn: "GuildMemberId",
                        onDelete: ReferentialAction.Cascade
                    );
                    table.ForeignKey(
                        name: "FK_GuildMemberMedals_Medals_MedalId",
                        column: x => x.MedalId,
                        principalTable: "Medals",
                        principalColumn: "MedalId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "PlayerHistoricalStats",
                columns: table => new
                {
                    StatHistoryId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WeekliesCompleted = table.Column<int>(type: "int", nullable: false),
                    WarsCompleted = table.Column<int>(type: "int", nullable: false),
                    HoursPlayed = table.Column<int>(type: "int", nullable: false),
                    SyncDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuildMemberId = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerHistoricalStats", x => x.StatHistoryId);
                    table.ForeignKey(
                        name: "FK_PlayerHistoricalStats_GuildMembers_GuildMemberId",
                        column: x => x.GuildMemberId,
                        principalTable: "GuildMembers",
                        principalColumn: "GuildMemberId"
                    );
                }
            );

            migrationBuilder.CreateTable(
                name: "RaidsCompleted",
                columns: table => new
                {
                    RaidCompletedId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaidId = table.Column<int>(type: "int", nullable: false),
                    RaidInstanceId = table.Column<int>(type: "int", nullable: false),
                    Uuid = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GuildMemberId = table.Column<int>(type: "int", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaidsCompleted", x => x.RaidCompletedId);
                    table.ForeignKey(
                        name: "FK_RaidsCompleted_GuildMembers_GuildMemberId",
                        column: x => x.GuildMemberId,
                        principalTable: "GuildMembers",
                        principalColumn: "GuildMemberId"
                    );
                    table.ForeignKey(
                        name: "FK_RaidsCompleted_Raids_RaidId",
                        column: x => x.RaidId,
                        principalTable: "Raids",
                        principalColumn: "RaidId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMemberGames_GameId",
                table: "GuildMemberGames",
                column: "GameId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMemberGames_GuildMemberId",
                table: "GuildMemberGames",
                column: "GuildMemberId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMemberMedals_GuildMemberId",
                table: "GuildMemberMedals",
                column: "GuildMemberId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMemberMedals_MedalId",
                table: "GuildMemberMedals",
                column: "MedalId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_GuildMembers_RankId",
                table: "GuildMembers",
                column: "RankId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_PlayerHistoricalStats_GuildMemberId",
                table: "PlayerHistoricalStats",
                column: "GuildMemberId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_GuildMemberId",
                table: "RaidsCompleted",
                column: "GuildMemberId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_RaidId",
                table: "RaidsCompleted",
                column: "RaidId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "Events");

            migrationBuilder.DropTable(name: "GuildMemberGames");

            migrationBuilder.DropTable(name: "GuildMemberMedals");

            migrationBuilder.DropTable(name: "PlayerHistoricalStats");

            migrationBuilder.DropTable(name: "RaidsCompleted");

            migrationBuilder.DropTable(name: "Games");

            migrationBuilder.DropTable(name: "Medals");

            migrationBuilder.DropTable(name: "GuildMembers");

            migrationBuilder.DropTable(name: "Raids");

            migrationBuilder.DropTable(name: "Ranks");
        }
    }
}
