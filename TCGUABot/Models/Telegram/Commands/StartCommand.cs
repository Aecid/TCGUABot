using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => "/start";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;
            var login = context.UserLogins.FirstOrDefault(l => l.LoginProvider == "Telegram" && l.ProviderKey == message.From.Id.ToString());
            if (login != null)
            {
                var user = context.Users.FirstOrDefault(u => u.Id == login.UserId);
                await client.SendTextMessageAsync(chatId, "Вы зарегистрированы как "+user.Email);
            }
            else
            {
                IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
                configurationBuilder.AddJsonFile("appsettings.json");
                IConfiguration configuration = configurationBuilder.Build();
                var baseUrl = configuration.GetSection("TelegramSettings").GetSection("TelegramWebHookHostBase").Value;
                var url = string.Format(configuration.GetSection("TelegramSettings").GetSection("TelegramWebHookHostBase").Value, "signin-telegram");

                var loginUrl = new LoginUrl() { BotUsername = "tcgua_bot", Url = url };

                InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(new InlineKeyboardButton()
                {
                    Text = "Логин на "+baseUrl,
                    LoginUrl = loginUrl
                });

                await client.SendTextMessageAsync(chatId, "Если хотите пользоваться функцией импорта деклистов, залогиньтесь на "+baseUrl+" через Телеграм.\r\nЕсли не хотите - не регистрируйтесь :)", replyMarkup: keyboard);
            }
        }
    }
}