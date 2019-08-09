using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
            string text = string.Empty;
            string setName = string.Empty;
            if (message.Text.Contains("(") && message.Text.Contains(")"))
            {
                var match = Regex.Match(message.Text, @"/c (.*)\((.*)\)");
                text = match.Groups[1].Value;
                setName = match.Groups[2].Value;
            }
            else
            { 
                text = message.Text.Replace("/c ", "");
            }
            var msg = string.Empty;
            var chatId = message.Chat.Id;
            Card card;
            if (setName != string.Empty)
            {
                card = Helpers.CardSearch.GetCardByName(text, setName);
            }
            else
            {
                card = Helpers.CardSearch.GetCardByName(text);
            }

            if ( card != null)
            {
                msg += "<b>🇺🇸" + card.name + "</b>\r\n";
                if (card.foreignData.Any(c => c.language.Equals("Russian"))) msg += "<b>🇷🇺" + card.ruName + "</b>\r\n";

                try
                {
                    var prices = GetCardPriceFromScryfallByMultiverseId(card.multiverseId);
                    if (prices.usd != null)
                        msg += "Цена: <b>$" + prices.usd.ToString() + "</b>\r\n";
                    if (prices.usd_foil != null)
                        msg += "Цена фойлы: <b>$" + prices.usd_foil.ToString() + "</b>\r\n";
                    if (prices.usd == null & prices.usd_foil == null)
                        msg += "Цена: <i>Нет данных о цене</i>\r\n";

                }
                catch
                { 
                    //if (card.prices.paper.Count > 0)
                    //{
                    //    var price = card.prices.paper.TakeLast(1).ToList()[0];
                    //    msg += "Цена на <b>" + price.Key + "</b>: <b>$" + price.Value.ToString() + "</b>\r\n";
                    //}

                }

                msg += "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card";
            }
            else
            {
                msg = "<b>❌Карта не найдена по запросу \""+text+"\".</b>";
            }

            await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html);
        }

        public dynamic GetCardPriceFromScryfallByMultiverseId(int multiverseId)
        {
            //https://api.scryfall.com/cards/multiverse/464166
            dynamic card = JsonConvert.DeserializeObject<dynamic>(CardData.ApiCall("https://api.scryfall.com/cards/multiverse/" + multiverseId));
            return card.prices;
        }
    }
}