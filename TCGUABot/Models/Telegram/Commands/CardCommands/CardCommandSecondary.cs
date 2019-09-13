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
    public class CardCommandSecondary : Command
    {
        public override string Name => "/c ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var tUser = message.From;
            Helpers.TelegramUtil.AddUser(tUser, context);

            var chatId = message.Chat.Id;
            if (message.Chat.Id == message.From.Id)
            {
                await client.SendTextMessageAsync(chatId, "❌К сожалению, @AskUrza_bot больше не поддерживает поиск карт в приватном чате.\r\n\r\nДля этих целей вы можете использовать @tcgua_bot.");
                return;
            }
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
                card = Helpers.CardSearch.GetCardByName(text.Trim(), setName);
            }
            else
            {
                card = Helpers.CardSearch.GetCardByName(text.Trim());
            }

            string nameEn = string.Empty;
            string nameRu = string.Empty;

            string price = string.Empty;
            if (card != null)
            {
                nameEn += "<b>🇺🇸 " + card.name + "</b>";
                if (card.foreignData.Any(c => c.language.Equals("Russian"))) nameRu += "<b>🇷🇺 " + card.ruName + "</b>";

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
                msg = "<b>❌Карта не найдена по запросу \"" + text + "\".</b>\r\nПопробуйте ввести в чат <b>\"@tcgua_bot имякарты\"</b> и подождать подсказку от бота.";
            }

            if (card != null)
            {
                if (card.names != null && card.names.Count > 0)
                {
                    nameEn = "<b>🇺🇸</b>";
                    nameRu = "<b>🇷🇺</b>";
                    var ComboList = new List<Card>();

                    foreach (var comboPiece in card.names)
                    {
                        var cpName = comboPiece.Trim();
                        Card secondCard;
                        secondCard = Helpers.CardSearch.GetCardByName(cpName, true);

                        nameEn += "|<b>" + comboPiece + "</b>";
                        if (secondCard.foreignData.Any(c => c.language.Equals("Russian"))) nameRu += "|<b>" + secondCard.ruName + "</b>";


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

                    var firstCardMuId = 0;
                    foreach (var foundCard in ComboList)
                    {
                        if (firstCardMuId != foundCard.multiverseId)
                        {
                            firstCardMuId = foundCard.multiverseId;
                            var imp = new InputMediaPhoto(new InputMedia("https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + foundCard.multiverseId + "&type=card"))
                            {
                                //Caption = foundCard.name
                            };
                            media.Add(imp);
                        }
                    }

                    foreach (var z in media)
                    {
                        Console.WriteLine(z.Caption);
                    }

                    int index = nameRu.IndexOf("|");
                    nameRu = (index < 0)
                        ? nameRu
                        : nameRu.Remove(index, 1);

                    index = nameEn.IndexOf("|");
                    nameEn = (index < 0)
                        ? nameEn
                        : nameEn.Remove(index, 1);

                    if (nameRu.Equals("<b>🇷🇺</b>")) msg = nameEn + "\r\n" + price;
                    else msg = nameEn + "\r\n" + nameRu + "\r\n" + price;

                    if (media.Count > 0)
                    {
                        await client.SendMediaGroupAsync(media, chatId);
                        await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html);
                    }

                }
                else
                {
                    msg += nameEn + "\r\n" + nameRu + (nameRu.Length > 0 ? "\r\n" : "") + price;
                    WebRequest req;
                    if (card.multiverseId > 0)
                        req = WebRequest.Create("https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card");
                    else
                        try
                        {
                            req = WebRequest.Create(CardData.GetTcgPlayerImage(card.tcgplayerProductId));
                        }
                        catch
                        {
                            req = null;
                        }

                    if (req != null)
                    {
                        using (Stream fileStream = req.GetResponse().GetResponseStream())
                        {
                            await client.SendPhotoAsync(chatId, new InputOnlineFile(fileStream), msg, Telegram.Bot.Types.Enums.ParseMode.Html);
                        }
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html);
                    }
                }
            }
            else
            {
                //msg = nameEn + "\r\n" + nameRu + "\r\n" + price;
                await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: message.MessageId);
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