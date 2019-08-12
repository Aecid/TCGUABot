using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

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
                await client.SendTextMessageAsync(chatId, "Залогиньтесь на https://ace.od.ua через Телеграм");
            }
        }
    }
}