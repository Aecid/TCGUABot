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
    }
}
