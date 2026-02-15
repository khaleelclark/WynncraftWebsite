using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Backend.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGuildMemberIdFromRaidCompleted : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaidsCompleted_GuildMembers_GuildMemberId",
                table: "RaidsCompleted"
            );

            migrationBuilder.AlterColumn<int>(
                name: "GuildMemberId",
                table: "RaidsCompleted",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int"
            );

            migrationBuilder.AddForeignKey(
                name: "FK_RaidsCompleted_GuildMembers_GuildMemberId",
                table: "RaidsCompleted",
                column: "GuildMemberId",
                principalTable: "GuildMembers",
                principalColumn: "GuildMemberId"
            );
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RaidsCompleted_GuildMembers_GuildMemberId",
                table: "RaidsCompleted"
            );

            migrationBuilder.AlterColumn<int>(
                name: "GuildMemberId",
                table: "RaidsCompleted",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true
            );

            migrationBuilder.AddForeignKey(
                name: "FK_RaidsCompleted_GuildMembers_GuildMemberId",
                table: "RaidsCompleted",
                column: "GuildMemberId",
                principalTable: "GuildMembers",
                principalColumn: "GuildMemberId",
                onDelete: ReferentialAction.Cascade
            );
        }
    }
}
