using Microsoft.EntityFrameworkCore.Migrations;

namespace TCGUABot.Data.Migrations
{
    public partial class guideaddformat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Format",
                table: "DeckGuides",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Format",
                table: "DeckGuides");
        }
    }
}
