using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImperialBackend.Migrations
{
    public partial class SetDefaultRankUponGuildMemberCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RankId",
                table: "GuildMembers",
                type: "int",
                nullable: false,
                defaultValue: 6,
                oldClrType: typeof(int),
                oldType: "int"
            );
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "RankId",
                table: "GuildMembers",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldDefaultValue: 6
            );
        }
    }
}
