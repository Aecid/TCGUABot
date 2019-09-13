using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using Telegram.Bot.Types;

namespace TCGUABot.Helpers
{
    public static class TelegramUtil
    {
        public static void AddUser(User telegramUser, ApplicationDbContext context)
        {
            try
            {
                var isExistingUser = context.TelegramUsers.Any(u => u.Id == telegramUser.Id);
                if (!isExistingUser)
                {
                    var user = new TelegramUser()
                    {
                        Id = telegramUser.Id,
                        FirstName = telegramUser.FirstName,
                        LastName = telegramUser.LastName,
                        Username = telegramUser.Username,
                        EmojiStatus = "🧙‍♂️"
                    };

                    context.TelegramUsers.Add(user);
                    context.SaveChanges();
                }
                else
                {
                    var existingUser = context.TelegramUsers.FirstOrDefault(u => u.Id == telegramUser.Id);
                    var areChanges = false;
                    if (existingUser.FirstName != telegramUser.FirstName)
                    {
                        areChanges = true;
                        existingUser.FirstName = telegramUser.FirstName;
                    }
                    if (existingUser.LastName != telegramUser.LastName)
                    {
                        areChanges = true;
                        existingUser.LastName = telegramUser.LastName;
                    }
                    if (existingUser.Username != telegramUser.Username)
                    {
                        areChanges = true;
                        existingUser.Username = telegramUser.Username;
                    }

                    if (areChanges)
                    {
                        context.SaveChanges();
                    }
                }
            }
            catch { }
        }

public static void AddChat(long chatId, ApplicationDbContext context, bool isPrivate=false)
        {
            try
            {
                var isExistingChat = context.TelegramChats.FirstOrDefault(c => c.Id == chatId);
                if (isExistingChat == null)
                {
                    TelegramChat chat = new TelegramChat();
                    chat.Id = chatId;
                    chat.Language = "ru";
                    if (isPrivate)
                        chat.SendSpoilers = false;
                    else chat.SendSpoilers = true;

                    context.TelegramChats.Add(chat);
                    context.SaveChanges();
                }
            }
            catch { }
        }
    }
}
