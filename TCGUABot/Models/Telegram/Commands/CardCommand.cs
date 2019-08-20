using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TCGUABot.Models.Commands
{
    public class CardCommand : Command
    {
        public override string Name => "/c ";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            string text = string.Empty;
            string setName = string.Empty;
            var originalMessage = message.Text.Substring(message.Text.IndexOf("/c "));
            if (originalMessage.Contains("(") && originalMessage.Contains(")"))
            {
                var match = Regex.Match(originalMessage, @"/c (.*)\((.*)\)");
                text = match.Groups[1].Value;
                setName = match.Groups[2].Value;
            }
            else
            { 
                text = originalMessage.Replace("/c ", "");
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
                card = Helpers.CardSearch.GetCardByName(text.Trim());
            }

            if ( card != null)
            {
                msg += "<b>🇺🇸" + card.name + "</b>\r\n";
                if (card.foreignData.Any(c => c.language.Equals("Russian"))) msg += "<b>🇷🇺" + card.ruName + "</b>\r\n";

                try
                {
                    var prices = CardData.GetTcgPlayerPrices(card.tcgplayerProductId);
                    if (prices["normal"] > 0)
                        msg += "Цена: <b>$" + prices["normal"].ToString() + "</b>\r\n";
                    if (prices["foil"] > 0)
                        msg += "Цена фойлы: <b>$" + prices["foil"].ToString() + "</b>\r\n";
                    if (prices["normal"] == 0 && prices["foil"] == 0)
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

                //msg += "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card";
            }
            else
            {
                msg = "<b>❌Карта не найдена по запросу \""+text+"\".</b>";
            }

            if (card != null)
            {
                var req = WebRequest.Create("https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card");

                using (Stream fileStream = req.GetResponse().GetResponseStream())
                {
                    await client.SendPhotoAsync(chatId, new InputOnlineFile(fileStream), msg, Telegram.Bot.Types.Enums.ParseMode.Html);
                }
            }
            else
            {
                await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId:message.MessageId);
            }
        }

        public dynamic GetCardPriceFromScryfallByMultiverseId(int multiverseId)
        {
            //https://api.scryfall.com/cards/multiverse/464166
            dynamic card = JsonConvert.DeserializeObject<dynamic>(CardData.ApiCall("https://api.scryfall.com/cards/multiverse/" + multiverseId));
            return card.prices;
        }
    }
}