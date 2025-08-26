using Microsoft.EntityFrameworkCore.Migrations;

namespace ImperialBackend.Migrations
{
    public partial class RemovePlayerSkinFromGuildMember : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PlayerSkin",
                table: "GuildMembers");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PlayerSkin",
                table: "GuildMembers",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
