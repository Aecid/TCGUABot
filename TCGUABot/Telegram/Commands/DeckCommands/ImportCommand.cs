using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using TCGUABot.Controllers;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class ImportCommand : Command
    {
        public override string Name => "/import";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var text = message.Text.Replace("/import ", "");
            var deck = ImportDeck.StringToDeck(text, null);

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
                deck.Owner = context.Users.FirstOrDefault(u => u.Id == "d34f08f5-9daa-46d6-a87c-cc3a6fda538a");
            }
            else
            {
                deck.Owner = context.Users.FirstOrDefault(u => u.Id == telegramUser.UserId);
            }

            var deckListController = new DecklistController(context);
            if (deck.MainDeck.Count > 0)
            {
                try
                {
                    var id = deckListController.Import(deck);
                    var link = string.Format(baseAddress, "Decks/Details?id=" + id);
                    msg += "Ссылка на деку: https://t.me/iv?url=" + HttpUtility.UrlEncode(link) + "&rhash=32913b8ff178b0";
                }
                catch
                {
                    msg = "❌Ошибка импорта деки.";
                }

            }
            else
            {
                msg = "❌Ошибка импорта деки.";
            }
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