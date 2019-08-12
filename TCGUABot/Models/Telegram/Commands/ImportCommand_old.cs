using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class OldImportCommand : Command
    {
        public override string Name => "/oldimport";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var text = message.Text.Replace("/import ", "");
            var deck = new DeckArenaImport();
            deck.MainDeck = new List<ArenaCard>();
            deck.SideBoard = new List<ArenaCard>();
            bool side = false;
            foreach (var myString in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                ArenaCard card = new ArenaCard();

                if (myString.Trim().Length > 1)
                {
                    var cardData = myString.Split(" ");
                    Regex exp = new Regex(@"(\d+)\s+(.*)\s+(\(.+\))\s+(\d+)");
                    var matches = exp.Matches(myString);
                
                    int.TryParse(matches[0].Groups[1].Value, out card.count);
                    card.name = matches[0].Groups[2].Value;
                    card.set = matches[0].Groups[3].Value.Replace("DAR", "DOM");
                    int.TryParse(matches[0].Groups[4].Value, out card.collectorNumber);

                    if (side) deck.SideBoard.Add(card);
                    else deck.MainDeck.Add(card);
                }

                else
                {
                    side = true;
                }
            }

            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.PostAsJsonAsync("/decklist", deck);
            response.EnsureSuccessStatusCode();

            var id = response.Content.ToString();

            //var controller = new TCGUABot.Controllers.DecklistController();
            //var id = controller.Import(deck);

            var chatId = message.Chat.Id;

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();
            var link = string.Format(configuration.GetSection("TelegramSettings").GetSection("TelegramWebHookHostBase").Value, "decks/"+id);
            //https://ace.od.ua:8443/decks/"+id
            await client.SendTextMessageAsync(chatId, "Ссылка на деку: "+link);
        }
    }
}