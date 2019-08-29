using Microsoft.EntityFrameworkCore.Migrations;

namespace TCGUABot.Data.Migrations
{
    public partial class TourneyUpdate2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "PlayerDeckPairs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "PlayerDeckPairs",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Username",
                table: "PlayerDeckPairs",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "PlayerDeckPairs");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "PlayerDeckPairs");

            migrationBuilder.DropColumn(
                name: "Username",
                table: "PlayerDeckPairs");
        }
    }
}
