using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImperialBackend.Migrations
{
    public partial class AddRaidInstancesAndRemoveRaidCompletedUuid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop old indexes that referenced columns we're deleting
            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_Uuid",
                table: "RaidsCompleted");

            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_CompletedDate_Uuid",
                table: "RaidsCompleted");

            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_RaidInstanceId",
                table: "RaidsCompleted");

            // Remove old model columns from RaidsCompleted (data can be discarded)
            migrationBuilder.DropColumn(
                name: "Uuid",
                table: "RaidsCompleted");

            migrationBuilder.DropColumn(
                name: "RaidInstanceId",
                table: "RaidsCompleted");

            // New table: RaidInstances
            migrationBuilder.CreateTable(
                name: "RaidInstances",
                columns: table => new
                {
                    RaidInstanceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RaidCompletedId = table.Column<int>(nullable: false),
                    GuildMemberId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RaidInstances", x => x.RaidInstanceId);
                    table.ForeignKey(
                        name: "FK_RaidInstances_RaidsCompleted_RaidCompletedId",
                        column: x => x.RaidCompletedId,
                        principalTable: "RaidsCompleted",
                        principalColumn: "RaidCompletedId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RaidInstances_GuildMembers_GuildMemberId",
                        column: x => x.GuildMemberId,
                        principalTable: "GuildMembers",
                        principalColumn: "GuildMemberId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "UX_RaidInstances_RaidCompletedId_GuildMemberId",
                table: "RaidInstances",
                columns: new[] { "RaidCompletedId", "GuildMemberId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RaidInstances_GuildMemberId",
                table: "RaidInstances",
                column: "GuildMemberId");

            migrationBuilder.CreateIndex(
                name: "IX_RaidInstances_RaidCompletedId",
                table: "RaidInstances",
                column: "RaidCompletedId");

            // New/updated indexes for new model
            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_CompletedDate",
                table: "RaidsCompleted",
                column: "CompletedDate");

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_RaidId",
                table: "RaidsCompleted",
                column: "RaidId");

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_RaidId_CompletedDate",
                table: "RaidsCompleted",
                columns: new[] { "RaidId", "CompletedDate" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop new indexes we added on RaidsCompleted
            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_CompletedDate",
                table: "RaidsCompleted");

            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_RaidId",
                table: "RaidsCompleted");

            migrationBuilder.DropIndex(
                name: "IX_RaidsCompleted_RaidId_CompletedDate",
                table: "RaidsCompleted");

            migrationBuilder.DropTable(
                name: "RaidInstances");

            // Re-add old columns (non-nullable, since your old schema was non-nullable)
            migrationBuilder.AddColumn<Guid>(
                name: "Uuid",
                table: "RaidsCompleted",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "RaidInstanceId",
                table: "RaidsCompleted",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Recreate old indexes
            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_Uuid",
                table: "RaidsCompleted",
                column: "Uuid");

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_CompletedDate_Uuid",
                table: "RaidsCompleted",
                columns: new[] { "CompletedDate", "Uuid" });

            migrationBuilder.CreateIndex(
                name: "IX_RaidsCompleted_RaidInstanceId",
                table: "RaidsCompleted",
                column: "RaidInstanceId");
        }
    }
}
