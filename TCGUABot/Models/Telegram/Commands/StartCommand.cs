using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class StartCommand : Command
    {
        public override string Name => "/start";

        public override async void Execute(Message message, TelegramBotClient client)
        {

            var chatId = message.Chat.Id;
            await client.SendTextMessageAsync(chatId, "/start yourself!");
        }
    }
}