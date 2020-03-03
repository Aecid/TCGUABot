using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace TCGUABot.Data.Migrations
{
    public partial class Ordertable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    PlayerTelegramId = table.Column<long>(nullable: true),
                    ProductId = table.Column<int>(nullable: true),
                    ProductName = table.Column<string>(nullable: true),
                    Edition = table.Column<string>(nullable: true),
                    EditionId = table.Column<int>(nullable: false),
                    isFoil = table.Column<bool>(nullable: false),
                    Lang = table.Column<string>(nullable: true),
                    Wts = table.Column<bool>(nullable: false),
                    Location = table.Column<string>(nullable: true),
                    isDeliverable = table.Column<bool>(nullable: false),
                    DeliveryService = table.Column<string>(nullable: true),
                    Description = table.Column<string>(nullable: true),
                    Quantity = table.Column<string>(nullable: true),
                    isOpen = table.Column<bool>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastModified = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
