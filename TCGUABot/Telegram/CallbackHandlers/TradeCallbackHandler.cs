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

            Console.WriteLine("|||" + query.Data + "|||");
            var dataArray = query.Data.Split("|");
            var type = dataArray[1];
            var name = dataArray[3];
            int productId;

            Console.WriteLine("{0},{1},{2},{3}", dataArray[0], dataArray[1], dataArray[2], dataArray[3]);



            if (!int.TryParse(dataArray[2], out productId)) return;

            Console.WriteLine("{0},{1},{2},{3}", dataArray[0], type, productId.ToString(), name);


            if (type == "wtb")
            {
                try
                {
                    await client.AnswerCallbackQueryAsync(query.Id, "Check your private messages");
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                try
                {
                    var command = new WtbCommand();
                    await command.Execute(new Message() { Text = "/wtb " + name, Chat = new Chat() { Id = query.From.Id } }, await Bot.Get(), context);

                    if (query.Message.Chat.Id == query.From.Id)
                    {
                        var msg = query.Message.Caption + "\r\n#покупка";
                        await client.EditMessageCaptionAsync(query.Message.Chat.Id, query.Message.MessageId, msg, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                return;
            }

            if (type == "wts")
            {
                try
                {
                    if (query.Message.Chat.Id == query.From.Id)
                    {
                        var msg = query.Message.Caption + "\r\n#продажа";
                        await client.EditMessageCaptionAsync(query.Message.Chat.Id, query.Message.MessageId, msg, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            }
        }
    }
}
