using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using TCGUABot.Models.CallbackHandlers;
using TCGUABot.Models.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.CallbackHandlers
{
    public class TradeCallbackHandler : CallbackHandler
    {
        public override string Name => "trade";

        public override async Task Execute(CallbackQuery query, TelegramBotClient client, ApplicationDbContext context)
        {
            var dataArray = query.Data.Split("|");
            var name = dataArray[0];
            var type = dataArray[1];
            int productId;

            if (!int.TryParse(dataArray[2], out productId)) return;
            try
            {
                await client.AnswerCallbackQueryAsync(query.Id, "Вы нажали \"" + type + "\", но функция пока не поддерживается :)");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
