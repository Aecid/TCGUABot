using Microsoft.EntityFrameworkCore.Migrations;

namespace TCGUABot.Data.Migrations
{
    public partial class Latlongtourney : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isPromo",
                table: "TradingCards",
                newName: "IsPromo");

            migrationBuilder.AddColumn<string>(
                name: "Language",
                table: "TradingCards",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Latitude",
                table: "Tournaments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LocationName",
                table: "Tournaments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Longitude",
                table: "Tournaments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Language",
                table: "TradingCards");

            migrationBuilder.DropColumn(
                name: "Latitude",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "LocationName",
                table: "Tournaments");

            migrationBuilder.DropColumn(
                name: "Longitude",
                table: "Tournaments");

            migrationBuilder.RenameColumn(
                name: "IsPromo",
                table: "TradingCards",
                newName: "isPromo");
        }
    }
}
