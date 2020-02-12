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
    public class CatifyCommand : Command
    {
        public override string Name => "/catify";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            if (message.From.Id == 186070199)
            {
                var chatId = message.Chat.Id;
                if (message.ReplyToMessage != null)
                {
                    var replyMessage = message.ReplyToMessage;
                    context.CatifiedUsers.Add(new CatifiedUser { TelegramId = replyMessage.From.Id });
                    context.SaveChanges();
                    await client.SendTextMessageAsync(chatId, "...Meow...", replyToMessageId:message.MessageId);
                }
            }
        }
    }
}