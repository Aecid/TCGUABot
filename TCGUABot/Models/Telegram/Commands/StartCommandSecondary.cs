using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class StartCommandSecondary : Command
    {
        public override string Name => "/start";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;
                await client.SendTextMessageAsync(chatId, "Это демо-версия бота с урезанным функционалом.\r\nПолная версия - c кастомным именем бота, рекламой, уведомлениями игроков в личные сообщения о предстоящих эвентах, регистрацией на турниры, сохранением и экспортом дек доступна по сезонному абонементу от разработчиков бота. \r\nЕсли вы заинтересованы - обращайтесь к @aecid. ");
        }
    }
}