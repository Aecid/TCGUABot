using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class CardOracleCommand : Command
    {
        public override string Name => "/oracle ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var text = message.Text.Replace("/oracle ", "");
            var msg = string.Empty;
            var chatId = message.Chat.Id;
            var card = Helpers.CardSearch.GetCardByName(text);

            var lang = context.TelegramChats.FirstOrDefault(f => f.Id == chatId)?.Language;
            lang = lang == null ? "ru" : lang;

            if (card != null)
            {
                msg += "<b>"+ Lang.Res(lang).enFlag + card.name + "</b>\r\n";
                if (card.foreignData.Any(c => c.language.Equals("Russian"))) msg += "<b>"+ Lang.Res(lang).ruFlag + card.ruName + "</b>";
                msg += "\r\n<b>" + card.type + "</b>";
                msg += "\r\n<b>" + card.manaCost + "</b>";
                msg += "\r\n"+card.text;
                if (!string.IsNullOrEmpty(card.power) && !string.IsNullOrEmpty(card.toughness))
                msg += "\r\n<b>" + card.power + " / " + card.toughness + "</b>";

            }
            else
            {
                msg = "<b>❌"+ Lang.Res(lang).cardNotFoundByRequest + " \""+text+"\"</b>";
            }

            await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}