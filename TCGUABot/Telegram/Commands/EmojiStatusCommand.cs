using NeoSmart.Unicode;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TCGUABot.Models.Commands
{
    public class EmojiStatusCommand : Command
    {
        public override string Name => "/status";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var emoji = message.Text.Replace("/status ", "");
            if (Emoji.IsEmoji(emoji, 1) || Emoji.All.ToList().Any(e => e.Sequence.AsString == emoji))
            {
                var tUser = message.From;
                Helpers.TelegramUtil.AddUser(tUser, context);

                var telegramUser = context.TelegramUsers.FirstOrDefault(u => u.Id == message.From.Id);
                telegramUser.EmojiStatus = emoji;
                context.SaveChanges();
                var chatId = message.Chat.Id;
                await client.SendTextMessageAsync(chatId, "From now you will be known as " + telegramUser.EmojiStatus + telegramUser.Name + "!", replyToMessageId:message.MessageId);
            }
            else
            {
                var chatId = message.Chat.Id;

                if (emoji == "/status")
                {
                    await client.SendTextMessageAsync(chatId, "Usage: /status {single emoji}", replyToMessageId: message.MessageId);
                }
                else
                {
                    await client.SendTextMessageAsync(chatId, "Probably \"" + emoji + "\" is not a single emoji.", replyToMessageId: message.MessageId);
                }
            }
        }
    }
}