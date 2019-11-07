using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class CardOracleRuCommand : Command
    {
        public override string Name => "/оракл ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var text = message.Text.Replace("/оракл ", "");
            var msg = string.Empty;
            var chatId = message.Chat.Id;
            var card = Helpers.CardSearch.GetCardByName(text);

            if (card != null)
            {
                bool isRussian = false;
                ForeignData ruData = new ForeignData();
                if (card.foreignData.Any(c => c.language.Equals("Russian")))
                {
                    ruData = card.foreignData.FirstOrDefault(c => c.language.Equals("Russian"));
                    isRussian = true;
                }
                msg += "<b>🇺🇸" + card.name + "</b>\r\n";
                if (isRussian) msg += "<b>🇷🇺" + card.ruName + "</b>";
                var cardType = isRussian ? ruData.type : card.type;
                msg += "\r\n<b>" + cardType + "</b>";
                msg += "\r\n<b>" + card.manaCost + "</b>";
                var cardText = isRussian ? ruData.text : card.text;
                msg += "\r\n"+cardText;
                if (!string.IsNullOrEmpty(card.power) && !string.IsNullOrEmpty(card.toughness))
                msg += "\r\n<b>" + card.power + " / " + card.toughness + "</b>";
            }
            else
            {
                msg = "<b>❌Карта не найдена.</b>";
            }
            if (chatId == -1001330824758) msg = msg.Replace("🇷🇺", "🏳‍🌈");

            await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}