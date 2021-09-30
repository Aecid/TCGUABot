using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    public class CardCommand : Command
    {
        public override string Name => "/c ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var tUser = message.From;
            Helpers.TelegramUtil.AddUser(tUser, context);

            var chatId = message.Chat.Id;
            try
            {
                await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadPhoto);
            }
            catch
            {

            }
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
            Card card = null;
            if (setName != string.Empty)
            {
                try
                {
                    card = Helpers.CardSearch.GetCardByName(text.Trim(), setName);
                }
                catch
                { }
            }
            else
            {
                card = Helpers.CardSearch.GetCardByName(text.Trim());
            }

            string nameEn = string.Empty;
            string nameRu = string.Empty;
            string set = string.Empty;
            var lang = context.TelegramChats.FirstOrDefault(f => f.Id == chatId)?.Language;
            lang = lang == null ? "ru" : lang;


            string price = string.Empty;
            if (card != null)
            {
                nameEn += "<b>" + Lang.Res(lang).enFlag + " " + card.name + "</b>";
                if (card.foreignData.Any(c => c.language.Equals("Russian"))) nameRu += "<b>" + Lang.Res(lang).ruFlag + " " + card.ruName + " </b>";
                set = "<i>(" + CardData.Instance.Sets.FirstOrDefault(s => s.code.Equals(card.Set, StringComparison.InvariantCultureIgnoreCase)).name + ")</i>";


                try
                {
                    var prices = CardData.GetTcgPlayerPrices(card.tcgplayerProductId);
                    if (prices["normal"] > 0)
                        price += Lang.Res(lang).price + ": <b>$" + prices["normal"].ToString() + "</b>\r\n";
                    if (prices["foil"] > 0)
                        price += Lang.Res(lang).priceFoil + ": <b>$" + prices["foil"].ToString() + "</b>\r\n";
                    if (prices["normal"] == 0 && prices["foil"] == 0)
                        price += Lang.Res(lang).price + ": <i>" + Lang.Res(lang).priceNoData + "</i>\r\n";

                }
                catch
                {
                }
            }
            else
            {
                msg = "<b>❌" + Lang.Res(lang).cardNotFoundByRequest + " \"" + text + "\".</b>\r\n" + Lang.Res(lang).tryAtTcgua + ".";
            }

            if (card != null)
            {
                if (card.otherFaceIds != null && card.otherFaceIds.Count > 0)
                {
                    var secondCard = Helpers.CardSearch.GetCardByMTGJsonUUID(card.otherFaceIds[0]);

                }
                else if (card.names != null && card.names.Count > 0)
                {
                    nameEn = "<b>" + Lang.Res(lang).enFlag + "</b>";
                    nameRu = "<b>" + Lang.Res(lang).ruFlag + "</b>";
                    set = "<i>(" + CardData.Instance.Sets.FirstOrDefault(s => s.code == card.Set).name + ")</i>";

                    var ComboList = new List<Card>();

                    foreach (var comboPiece in card.names)
                    {
                        var cpName = comboPiece.Trim();
                        Card secondCard;
                        secondCard = Helpers.CardSearch.GetCardByName(cpName, true);
                        nameEn += "|<b>" + comboPiece + "</b>";

                        if (secondCard != null)
                        {
                            if (secondCard.foreignData.Any(c => c.language.Equals("Russian"))) nameRu += "|<b>" + secondCard.ruName + "</b>";
                            ComboList.Add(secondCard);
                        }

                    }

                    List<IAlbumInputMedia> media = new List<IAlbumInputMedia>();

                    var firstCardMuId = 0;
                    var firstCardTcgPlayerId = 0;
                    foreach (var foundCard in ComboList)
                    {
                        if (foundCard.scryfallId != null)
                        {
                            var imp = new InputMediaPhoto(new InputMedia("https://c1.scryfall.com/file/scryfall-cards/large/front/" + foundCard.scryfallId[0] + "/" + foundCard.scryfallId[1] + "/" + foundCard.scryfallId + ".jpg"))
                            {
                                //Caption = foundCard.name
                            };
                            media.Add(imp);
                        }
                        //https://c1.scryfall.com/file/scryfall-cards/large/front/a/8/a8dbb9aa-1bf8-447d-a96c-33e2248bfb01.jpg
                        else if (foundCard.multiverseId > 0)
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
                        else
                        {
                            if (firstCardTcgPlayerId != foundCard.tcgplayerProductId)
                            {
                                firstCardTcgPlayerId = foundCard.tcgplayerProductId;
                                var imp = new InputMediaPhoto(new InputMedia(context.Cards.FirstOrDefault(c => c.ProductId == firstCardTcgPlayerId).ImageUrl));
                                media.Add(imp);
                            }
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

                    if (nameRu.Equals("<b>" + Lang.Res(lang).ruFlag + "</b>")) msg = nameEn + "\r\n" + set + "\r\n" + price;
                    else msg = nameEn + "\r\n" + nameRu + "\r\n" + set + "\r\n" + price;

                    if (media.Count > 0)
                    {
                        var buttonsList = new List<InlineKeyboardButton>();
                        //buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTB", "trade|wtb|" + card.tcgplayerProductId + "|" + card.name));
                        //buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTS", "trade|wts|" + card.tcgplayerProductId + "|" + card.name));
                        var keyboard = new InlineKeyboardMarkup(buttonsList);

                        await client.SendMediaGroupAsync(media, chatId);
                        await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard);
                    }

                }
                else
                {
                    msg += nameEn + "\r\n" + nameRu + (nameRu.Length > 0 ? "\r\n" : "") + set + "\r\n" + price;
                    WebRequest req;

                    try
                    {
                        req = WebRequest.Create("https://c1.scryfall.com/file/scryfall-cards/large/front/" + card.scryfallId[0] + "/" + card.scryfallId[1] + "/" + card.scryfallId + ".jpg");
                    }
                    catch
                    {
                        req = null;
                    }

                    if (req == null)
                    {
                        try
                        {
                            req = WebRequest.Create("https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card");
                        }
                        catch
                        {
                            req = null;
                        }
                    }
                    if (req == null)
                    {
                        try
                        {
                            req = WebRequest.Create(CardData.GetTcgPlayerImage(card.tcgplayerProductId));
                        }
                        catch
                        {
                            req = null;
                        }
                    }

                    if (req != null)
                    {
                        try
                        {
                            using (Stream fileStream = req.GetResponse().GetResponseStream())
                            {

                                //var buttonsList = new List<InlineKeyboardButton>();
                                ////buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTB", "trade|wtb|" + card.tcgplayerProductId + "|" + card.name));
                                ////buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTS", "trade|wts|" + card.tcgplayerProductId + "|" + card.name));
                                //var keyboard = new InlineKeyboardMarkup(buttonsList);

                                await client.SendPhotoAsync(chatId, new InputOnlineFile(fileStream), msg, Telegram.Bot.Types.Enums.ParseMode.Html);//, replyMarkup: keyboard);
                            }
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);

                            try
                            {
                                //var buttonsList = new List<InlineKeyboardButton>();
                                ////buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTB", "trade|wtb|" + card.tcgplayerProductId + "|" + card.name));
                                ////buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTS", "trade|wts|" + card.tcgplayerProductId + "|" + card.name));
                                //var keyboard = new InlineKeyboardMarkup(buttonsList);

                                req = WebRequest.Create(CardData.GetTcgPlayerImage(card.tcgplayerProductId));
                                using (Stream fileStream = req.GetResponse().GetResponseStream())
                                {
                                    await client.SendPhotoAsync(chatId, new InputOnlineFile(fileStream), msg, Telegram.Bot.Types.Enums.ParseMode.Html);//, replyMarkup: keyboard);
                                }
                            }
                            catch
                            {

                            }
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