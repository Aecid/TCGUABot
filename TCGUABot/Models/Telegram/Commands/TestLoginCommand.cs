using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    public class TestLoginCommand : Command
    {
        public override string Name => "/testLogin";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var chatId = message.Chat.Id;

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();
            var url = string.Format(configuration.GetSection("TelegramSettings").GetSection("TelegramWebHookHostBase").Value, "signin-telegram");

            var loginUrl = new LoginUrl() { BotUsername = "tcgua_bot", Url = url };

            InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton()
            {
                Text = "Login",
                LoginUrl = loginUrl
            });

            await client.SendTextMessageAsync(chatId, "Login button below", replyMarkup: keyboard);
        }
    }
}