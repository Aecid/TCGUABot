using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.CallbackHandlers
{
    public abstract class CallbackHandler
    {
        public abstract string Name { get; }
        public abstract Task Execute(CallbackQuery query, TelegramBotClient client, ApplicationDbContext context);
        public bool Is(string callbackData)
        {
            try
            {
                return (callbackData.Split("|")[0].Equals(this.Name));
            }
            catch
            {
                return false;
            }
        }
    }
}
