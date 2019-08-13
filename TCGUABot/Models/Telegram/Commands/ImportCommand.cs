using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Controllers;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class ImportCommand : Command
    {
        public override string Name => "/import";

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


            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();
            var baseAddress = configuration.GetSection("TelegramSettings").GetSection("TelegramWebHookHostBase").Value;
            //HttpClient httpClient = new HttpClient();
            //httpClient.BaseAddress = new Uri(baseAddress);
            //HttpResponseMessage response = await httpClient.PostAsJsonAsync("/Decklist/Import", deck);
            var msg = string.Empty;
            var telegramUser = context.UserLogins.FirstOrDefault(l => l.LoginProvider == "Telegram" && l.ProviderKey == message.From.Id.ToString());
            if (telegramUser == null)
            {
                msg += "Вы не зарегистрированы на сайте. Деклист будет импортирован временно и может быть удалён в любое время\r\n";
                deck.Owner = context.Users.FirstOrDefault(u => u.Id == "548ba8ce-d90a-4f33-834a-bc2a78372df6");
            }
            else
            {
                deck.Owner = context.Users.FirstOrDefault(u => u.Id == telegramUser.UserId);
            }

            var deckListController = new DecklistController(context);
            var id = deckListController.Import(deck);


            var link = string.Format(baseAddress, "decks/" + id);
            msg += "Ссылка на деку: " + link;
            //response.EnsureSuccessStatusCode();

            //var id = await response.Content.ReadAsStringAsync();

            //var controller = new TCGUABot.Controllers.DecklistController();
            //var id = controller.Import(deck);
            var chatId = message.Chat.Id;

            //https://ace.od.ua:8443/decks/"+id
            await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId:message.MessageId);
        }
    }
}