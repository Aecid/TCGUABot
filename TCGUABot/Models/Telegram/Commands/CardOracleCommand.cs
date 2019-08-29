using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class CardOracleCommand : Command
    {
        public override string Name => "/oracle ";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var text = message.Text.Replace("/oracle ", "");
            var msg = string.Empty;
            var chatId = message.Chat.Id;
            var card = Helpers.CardSearch.GetCardByName(text);

            if (card != null)
            {
                msg += "<b>🇺🇸" + card.name + "</b>\r\n";
                if (card.foreignData.Any(c => c.language.Equals("Russian"))) msg += "<b>🇷🇺" + card.ruName + "</b>\r\n";
                msg += "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card";
                msg += "\r\n" + card.type;
                msg += "\r\n" + card.manaCost;
                msg += "\r\n<b>Text:</b>\r\n";
                msg += card.text;
                if (!string.IsNullOrEmpty(card.power) && !string.IsNullOrEmpty(card.toughness))
                msg += "\r\n<b>" + card.power + "/" + card.toughness + "</b>";

            }
            else
            {
                msg = "<b>❌Карта не найдена.</b>";
            }

            await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}