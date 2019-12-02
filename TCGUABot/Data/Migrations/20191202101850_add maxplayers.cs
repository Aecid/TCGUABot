using Microsoft.EntityFrameworkCore.Migrations;

namespace TCGUABot.Data.Migrations
{
    public partial class addmaxplayers : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MaxPlayers",
                table: "Tournaments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MaxPlayers",
                table: "Tournaments");
        }
    }
}
