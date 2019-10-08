using Microsoft.EntityFrameworkCore.Migrations;

namespace TCGUABot.Data.Migrations
{
    public partial class addsomefieldstotelegramuserandchat : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AcceptBroadcast",
                table: "TelegramUsers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "SendTourneyReminder",
                table: "TelegramChats",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Decks",
                nullable: false,
                defaultValueSql: "uuid_generate_v4()",
                oldClrType: typeof(string));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AcceptBroadcast",
                table: "TelegramUsers");

            migrationBuilder.DropColumn(
                name: "SendTourneyReminder",
                table: "TelegramChats");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Decks",
                nullable: false,
                oldClrType: typeof(string),
                oldDefaultValueSql: "uuid_generate_v4()");
        }
    }
}
