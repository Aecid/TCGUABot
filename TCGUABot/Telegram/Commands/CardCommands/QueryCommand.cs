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
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    public class QueryCommand : Command
    {
        public override string Name => "/q ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var chatId = message.Chat.Id;

            var cardList = CardData.Names.Where(z => message.Text.Contains(" "+z+" ", StringComparison.InvariantCultureIgnoreCase) || message.Text.EndsWith(" "+z, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (cardList == null || cardList.Count == 0) return;

            foreach (var cardName in cardList)
            {
                var card = Helpers.CardSearch.GetCardByName(cardName, true);

                await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.Typing);

                var msg = string.Empty;
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

                        int index = nameRu.IndexOf("|");
                        nameRu = (index < 0)
                            ? nameRu
                            : nameRu.Remove(index, 1);

                        index = nameEn.IndexOf("|");
                        nameEn = (index < 0)
                            ? nameEn
                            : nameEn.Remove(index, 1);


                    }

                    if (nameRu.Equals("<b>🇷🇺</b>") || string.IsNullOrEmpty(nameRu)) msg = nameEn + "\r\n" + price;
                    else msg = nameEn + "\r\n" + nameRu + "\r\n" + price;

                    if (card.tcgplayerProductId > 0)
                    {
                        var buttonsList = new List<InlineKeyboardButton>();
                        buttonsList.Add(InlineKeyboardButton.WithCallbackData("👀", "cp|" + card.tcgplayerProductId));
                        buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTB", "trade|wtb|" + card.tcgplayerProductId));
                        //buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTS", "trade|wts|" + card.tcgplayerProductId));
                        var keyboard = new InlineKeyboardMarkup(buttonsList);

                        await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard);
                    }
                    else
                    {
                        await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html);
                    }
                }
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