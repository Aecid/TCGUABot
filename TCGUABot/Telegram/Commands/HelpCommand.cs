using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    public class HelpCommand : Command
    {
        public override string Name => "/help";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;
            var tUser = message.From;
            Helpers.TelegramUtil.AddUser(tUser, context);
            try
            {
                await client.SendTextMessageAsync(chatId, @"Привет!

Поиск карт по имени происходит через имя бота: введите, например, ""@tcgua_bot bolt"" и подождите подсказок от бота.
Цены в боте показываются по медиане TCGPlayer.

Чтоб искать карту конкретной редакции, введите ""@tcgua_bot {код редакции}:bolt"", например, ""@tcgua_bot m10:bolt""

Дополнительные доступные команды:
/c {cardname} - ищет карту по названию на русском или английском. Пример: ""/c lightning bolt"" (лучше использовать подсказки) 

/rs {cardname} - показывает дополнительные рулинги карты. Пример: ""/rs lich's mirror""

/oracle {cardname} - показывает текст карты и ссылку на картинку. Пример ""/oracle Tarmogoyf""

/оракл {cardname} - показывает русский текст карты если у карты есть перевод на русский. Пример ""/оракл Утечка Маны""

/tc {cardname} - показывает все редакции найденной карты и цены на них, например, ""/tc liliana of the veil""

/tc {set}:{*, or cardName} - показывает все карты с найденным названием в этом сете, отсортированные от дорогих к дешевым. Пример - ""/tc eld:oko"". Звёздочка - ""*"" - показывает топ 20 дорогих карт в этом сете, пример - ""/tc xln:*""
");
            }
            catch
            {

            }
        }
    }
}