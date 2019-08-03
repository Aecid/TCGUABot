using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class CardCommand : Command
    {
        public override string Name => "/c";

        public override async void Execute(Message message, TelegramBotClient client)
        {
            var text = message.Text.Replace("/c ", "");
            var msg = string.Empty;
            var chatId = message.Chat.Id;
            var card = Helpers.CardSearch.GetCardByName(text);

            if ( card != null)
            {
                msg += "<b>🇺🇸" + card.name + "</b>\r\n";
                if (card.foreignData.Any(c => c.language.Equals("Russian"))) msg += "<b>🇷🇺" + card.ruName + "</b>\r\n";
                if (card.prices.paper.Count > 0)
                {
                    var price = card.prices.paper.TakeLast(1).ToList()[0];
                    msg += "Цена на <b>" + price.Key + "</b>: <b>$" + price.Value.ToString() + "</b>\r\n";
                }
                msg += "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card";
            }
            else
            {
                msg = "<b>❌Карта не найдена.</b>";
            }

            await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html);
        }
    }
}