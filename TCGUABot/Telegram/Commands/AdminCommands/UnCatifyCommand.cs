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
    public class UnCatifyCommand : Command
    {
        public override string Name => "/uncatify";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            if (message.From.Id == 186070199)
            {
                var chatId = message.Chat.Id;
                if (message.ReplyToMessage != null)
                {
                    var replyMessage = message.ReplyToMessage;
                    var usr = context.CatifiedUsers.FirstOrDefault(z => z.TelegramId == replyMessage.From.Id);
                    context.CatifiedUsers.Remove(usr);
                    context.SaveChanges();
                    await client.SendTextMessageAsync(chatId, "...UnMeow...", replyToMessageId:message.MessageId);
                }
            }
        }
    }
}