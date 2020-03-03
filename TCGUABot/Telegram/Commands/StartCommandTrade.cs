using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class StartCommandTrade : Command
    {
        public override string Name => "/start";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;
                await client.SendTextMessageAsync(chatId, "Это альфа-тест бота для торговли картами.\r\n @mtg_tradebot {имякарты} - ждем подсказки от бота, выбираем карту какую хотим купить.");
        }
    }
}