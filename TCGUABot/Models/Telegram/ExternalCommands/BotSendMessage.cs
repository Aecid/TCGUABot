using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models.ExternalCommands
{
    public class BotSendMessage
    {
        public async void Execute(long chatId, string message)
        {
            var client = await Bot.Get();
            await client.SendTextMessageAsync(chatId, message, Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}
