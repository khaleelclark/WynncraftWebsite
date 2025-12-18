using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ImperialBackend.Migrations
{
    /// <inheritdoc />
    public partial class CorrectRenameSeasonRaitingToSeasonRating : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
            name: "SeasonRaiting",
            table: "Raids",
            newName: "SeasonRating");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
            name: "SeasonRaiting",
            table: "Raids",
            newName: "SeasonRating");

        }
    }
}
