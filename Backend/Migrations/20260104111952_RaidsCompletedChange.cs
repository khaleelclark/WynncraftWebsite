using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class RaidsCompletedChange : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaidsCompleted_GuildMembers_GuildMemberId",
                table: "RaidsCompleted"
            );

            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_CompletedDate_Uuid",
                table: "RaidsCompleted"
            );

            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_GuildMemberId",
                table: "RaidsCompleted"
            );

            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_RaidInstanceId",
                table: "RaidsCompleted"
            );

            migrationBuilder.DropIndex(name: "IX_RaidsCompleted_Uuid", table: "RaidsCompleted");

            migrationBuilder.DropIndex(name: "IX_GuildMembers_Uuid", table: "GuildMembers");

            migrationBuilder.DropColumn(name: "GuildMemberId", table: "RaidsCompleted");

            migrationBuilder.DropColumn(name: "RaidInstanceId", table: "RaidsCompleted");

            migrationBuilder.DropColumn(name: "Uuid", table: "RaidsCompleted");

            migrationBuilder.CreateTable(
                name: "RaidInstances",
                columns: table => new
                {
                    RaidInstanceId = table
                        .Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaidCompletedId = table.Column<int>(type: "int", nullable: false),
                    GuildMemberId = table.Column<int>(type: "int", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaidInstances", x => x.RaidInstanceId);
                    table.ForeignKey(
                        name: "FK_RaidInstances_GuildMembers_GuildMemberId",
                        column: x => x.GuildMemberId,
                        principalTable: "GuildMembers",
                        principalColumn: "GuildMemberId",
                        onDelete: ReferentialAction.Restrict
                    );
                    table.ForeignKey(
                        name: "FK_RaidInstances_RaidsCompleted_RaidCompletedId",
                        column: x => x.RaidCompletedId,
                        principalTable: "RaidsCompleted",
                        principalColumn: "RaidCompletedId",
                        onDelete: ReferentialAction.Cascade
                    );
                }
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_CompletedDate",
                table: "RaidsCompleted",
                column: "CompletedDate"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidInstances_GuildMemberId",
                table: "RaidInstances",
                column: "GuildMemberId"
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidInstances_RaidCompletedId",
                table: "RaidInstances",
                column: "RaidCompletedId"
            );

            migrationBuilder.CreateIndex(
                name: "UX_RaidInstances_RaidCompletedId_GuildMemberId",
                table: "RaidInstances",
                columns: new[] { "RaidCompletedId", "GuildMemberId" },
                unique: true
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "RaidInstances");

            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_CompletedDate",
                table: "RaidsCompleted"
            );

            migrationBuilder.AddColumn<int>(
                name: "GuildMemberId",
                table: "RaidsCompleted",
                type: "int",
                nullable: true
            );

            migrationBuilder.AddColumn<int>(
                name: "RaidInstanceId",
                table: "RaidsCompleted",
                type: "int",
                nullable: false,
                defaultValue: 0
            );

            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "RaidsCompleted",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000")
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_CompletedDate_Uuid",
                table: "RaidsCompleted",
                columns: new[] { "CompletedDate", "Uuid" }
            );

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_GuildMemberId",
                table: "RaidsCompleted",
                column: "GuildMemberId"
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
                name: "IX_GuildMembers_Uuid",
                table: "GuildMembers",
                column: "Uuid",
                unique: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_RaidsCompleted_GuildMembers_GuildMemberId",
                table: "RaidsCompleted",
                column: "GuildMemberId",
                principalTable: "GuildMembers",
                principalColumn: "GuildMemberId"
            );
        }
    }
}
