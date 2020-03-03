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

namespace TCGUABot.Models.Commands
{
    public class BroadcastCommand : Command
    {
        public override string Name => "/broadcast ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            if (message.From.Id == 186070199)
            {
                var chatId = message.Chat.Id;
                var text = message.Text.Replace("/broadcast ", "");
                text = "<b>Broadcast: </b>" + text;

                foreach (var user in context.TelegramUsers.Where(z => z.AcceptBroadcast == true && z.Id == 186070199))
                {
                    try
                    {
                        await Task.Delay(500);
                        await client.SendTextMessageAsync(user.Id, text, Telegram.Bot.Types.Enums.ParseMode.Html);
                    }
                    catch {
                        await client.SendTextMessageAsync("-1001112744433", "Error sending broadcast to user id "+user.Id, Telegram.Bot.Types.Enums.ParseMode.Html);
                    }
                }
            }
        }
    }
}