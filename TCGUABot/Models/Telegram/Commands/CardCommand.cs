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
            var chatId = message.Chat.Id;
            await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadPhoto);
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
            Card card;
            if (setName != string.Empty)
            {
                card = Helpers.CardSearch.GetCardByName(text, setName);
            }
            else
            {
                card = Helpers.CardSearch.GetCardByName(text.Trim());
            }

            string nameEn = string.Empty;
            string nameRu = string.Empty;

            string price = string.Empty;
            if ( card != null)
            {
                nameEn += "<b>🇺🇸" + card.name + "</b>";
                if (card.foreignData.Any(c => c.language.Equals("Russian"))) nameRu += "<b>🇷🇺" + card.ruName + "</b>";

                try
                {
                    var prices = CardData.GetTcgPlayerPrices(card.tcgplayerProductId);
                    if (prices["normal"] > 0)
                        price += "Цена: <b>$" + prices["normal"].ToString() + "</b>\r\n";
                    if (prices["foil"] > 0)
                        price += "Цена фойлы: <b>$" + prices["foil"].ToString() + "</b>\r\n";
                    if (prices["normal"] == 0 && prices["foil"] == 0)
                        price += "Цена: <i>Нет данных о цене</i>\r\n";

                }
                catch
                { 
                }
            }
            else
            {
                msg = "<b>❌Карта не найдена по запросу \""+text+"\".</b>";
            }

            if (card != null)
            {
                if (card.names != null)
                {
                    if (card.names.Count > 0) //if transform?
                    {
                        nameEn = "<b>🇺🇸</b>";
                        nameRu = "<b>🇷🇺</b>";
                        var ComboList = new List<Card>();

                        foreach (var comboPiece in card.names)
                        {
                            var cpName = comboPiece.Trim();
                            Card secondCard;
                            secondCard = Helpers.CardSearch.GetCardByName(cpName);

                            nameEn += "|<b>" + comboPiece + "</b>";
                            if (secondCard.foreignData.Any(c => c.language.Equals("Russian"))) nameRu += "|<b>" + card.ruName + "</b>";


                            if (secondCard != null)
                            {
                                Console.WriteLine("Card found");
                                ComboList.Add(secondCard);
                            }
                            else
                            {
                                Console.WriteLine("Not found");
                                msg += "❌Карта " + cpName + " была не найдена\r\n";
                            }
                        }

                        List<IAlbumInputMedia> media = new List<IAlbumInputMedia>();
                        foreach (var foundCard in ComboList)
                        {
                            var imp = new InputMediaPhoto(new InputMedia("https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + foundCard.multiverseId + "&type=card"))
                            {
                                Caption = foundCard.name
                            };
                            media.Add(imp);
                        }

                        foreach (var z in media)
                        {
                            Console.WriteLine(z.Caption);
                        }

                        msg = nameEn + "\r\n" + nameRu + "\r\n" + price;
                        if (media.Count > 0)
                        {
                            await client.SendMediaGroupAsync(media, chatId);
                            await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html);
                        }
                    }
                    else
                    {
                        msg += nameEn + "\r\n" + nameRu + "\r\n" + price;
                        var req = WebRequest.Create("https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card");

                        using (Stream fileStream = req.GetResponse().GetResponseStream())
                        {
                            await client.SendPhotoAsync(chatId, new InputOnlineFile(fileStream), msg, Telegram.Bot.Types.Enums.ParseMode.Html);
                        }
                    }
                }
            }
            else
            {
                msg = nameEn + "\r\n" + nameRu + "\r\n" + price;
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