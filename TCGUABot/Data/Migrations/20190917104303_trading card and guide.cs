using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TCGUABot.Data.Migrations
{
    public partial class tradingcardandguide : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeckGuides",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Keywords = table.Column<string[]>(nullable: true),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeckGuides", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TradingCards",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ProductId = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: true),
                    Set = table.Column<string>(nullable: true),
                    isPromo = table.Column<bool>(nullable: false),
                    IsFoil = table.Column<bool>(nullable: false),
                    Notes = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    State = table.Column<string>(nullable: true),
                    OwnerTelegramId = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TradingCards", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeckGuides");

            migrationBuilder.DropTable(
                name: "TradingCards");
        }
    }
}
