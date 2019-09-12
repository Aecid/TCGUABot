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
    public class StartCommand : Command
    {
        public override string Name => "/start";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;
            var tUser = message.From;
            Helpers.TelegramUtil.AddUser(tUser, context);
            await client.SendTextMessageAsync(chatId, @"Привет!

Поиск карт по имени происходит через имя бота: введите, например, ""@tcgua_bot bolt"" и подождите подсказок от бота.
Цены в боте показываются по медиане TCGPlayer.

Доступные команды:
/c {cardname} - ищет карту по названию на русском или английском. Пример: ""/c lightning bolt""

/rs {cardname} - показывает дополнительные рулинги карты. Пример: ""/rs lich's mirror""

/oracle {cardname} - показывает текст карты и ссылку на картинку. Пример ""/oracle Tarmogoyf""

/оракл {cardname} - показывает русский текст карты если у карты есть перевод на русский. Пример ""/оракл Утечка Маны""

/tourney - показывает анонсированные турниры в Одессе (турниры в других локациях в разработке)

/import {deckImport} - вы можете импортировать деку из MTG Arena и сохранить её на сайте. Если вы не зарегистрированы на сайте или не связали аккаунт на сайте с аккаунтом телеграмма, дека может быть удалена в любой момент

/login - авторизирует пользователя на сайте и создает аккаунт привязанный к телеграмму");
        }
    }
}