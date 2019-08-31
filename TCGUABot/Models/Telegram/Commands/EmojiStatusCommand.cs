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

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var emoji = message.Text.Replace("/status ", "");
            if (Emoji.IsEmoji(emoji, 1) || Emoji.All.ToList().Any(e => e.Sequence.AsString == emoji))
            {
                var isExistingUser = context.TelegramUsers.Any(u => u.Id == message.From.Id);
                if (!isExistingUser)
                {
                    var user = new TelegramUser()
                    {
                        Id = message.From.Id,
                        FirstName = message.From.FirstName,
                        LastName = message.From.LastName,
                        Username = message.From.Username,
                        EmojiStatus = "🧙‍♂️"

                    };

                    context.TelegramUsers.Add(user);
                    context.SaveChanges();
                }
                else
                {
                    var existingUser = context.TelegramUsers.FirstOrDefault(u => u.Id == message.From.Id);
                    var areChanges = false;
                    if (existingUser.FirstName != message.From.FirstName)
                    {
                        areChanges = true;
                        existingUser.FirstName = message.From.FirstName;
                    }
                    if (existingUser.LastName != message.From.LastName)
                    {
                        areChanges = true;
                        existingUser.LastName = message.From.LastName;
                    }
                    if (existingUser.Username != message.From.Username)
                    {
                        areChanges = true;
                        existingUser.Username = message.From.Username;
                    }

                    if (areChanges)
                    {
                        context.SaveChanges();
                    }
                }

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