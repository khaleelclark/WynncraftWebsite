using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImperialBackend.Migrations
{
    /// <inheritdoc />
    public partial class AddLastSyncedToGuildMember : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastSynced",
                table: "GuildMembers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastSynced",
                table: "GuildMembers");
        }
    }
}
