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
    public class CardToPrivateMessageCallbackHandler : CallbackHandler
    {
        public override string Name => "cp";

        public override async Task Execute(CallbackQuery query, TelegramBotClient client, ApplicationDbContext context)
        {
            var dataArray = query.Data.Split("|");
            var name = dataArray[0];
            int productId;

            if (!int.TryParse(dataArray[1], out productId)) return;

            try
            {
                await client.AnswerCallbackQueryAsync(query.Id, "Карта отправлена в приват!", false);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }


            var card = Helpers.CardSearch.GetCardByTcgPlayerProductId(productId);


            var chatId = query.From.Id;

            var command = new CardCommand();
            var message = new Message()
            {
                Text = "/c " + card.name + "(" + card.Set + ")",
                Chat = new Chat()
                {
                    Id = chatId
                }
            };

            await command.Execute(message, await Bot.Get(), context);
        }
    }
}
