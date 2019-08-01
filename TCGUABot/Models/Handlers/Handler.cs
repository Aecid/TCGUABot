using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Handlers
{
    public abstract class Handler
    {
        public abstract string Name { get; }
        public abstract void Execute(CallbackQuery query, TelegramBotClient client);
        public bool Is(string callbackData)
        {
            try
            {
                return (callbackData.Split("_")[0].Equals(this.Name));
            }
            catch
            {
                return false;
            }
        }
    }
}
