using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
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
    //TODO switch from CardCommand to this
    public class TestCardCommand : Command
    {
        public override string Name => "/tc";

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
            var originalMessage = message.Text.Substring(message.Text.IndexOf("/tc")).Trim().Replace("@tcgua_bot", "");
            if (originalMessage.Contains("(") && originalMessage.Contains(")"))
            {
                var match = Regex.Match(originalMessage, @"/tc (.*)\((.*)\)");
                text = match.Groups[1].Value;
                setName = match.Groups[2].Value;
            }
            else
            {
                text = originalMessage.Replace("/tc", "").Trim();
            }


            var lang = context.TelegramChats.FirstOrDefault(f => f.Id == chatId)?.Language;
            lang = lang == null ? "ru" : lang;
            var msg = string.Empty;


            var cards = new List<Product>();


            if (text.Contains(":"))
            {
                var a = text.Split(":");
                var set = a[0].Trim();
                var card = a[1].Trim();

                if (set.Length > 2 && (card.Length > 2 || card.Equals("*") || card.Equals("**")))
                {
                    var results = GetCardsFromDBBySet(card, set, context);

                    try
                    {
                        if (results.Count > 0)
                        {
                            cards = results;
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + "|" + e?.InnerException?.Message);
                    }
                }
            }
            else
            {

                var nonSupplemental = CardData.TcgGroups.Where(z => z.isSupplemental = false);
                var supplemental = CardData.TcgGroups.Where(z => z.isSupplemental = true);

                var cardsContains = context.Cards.Where(c => c.Name.ToLower().Contains(text.ToLower())).ToList();

                var cardsFull = cardsContains.Where(c => c.Name.ToLower() == text.ToLower()).ToList();
                cardsContains = cardsContains.Except(cardsFull).ToList();
                var cardsStartsWith = cardsContains.Where(c => c.Name.ToLower().StartsWith(text.ToLower())).ToList();
                cardsContains = cardsContains.Except(cardsStartsWith).ToList();

                cards.AddRange(cardsFull);
                cards.AddRange(cardsStartsWith);
                cards.AddRange(cardsContains);

            }


            var total = cards.Count();

            if (cards.Count > 1)
            {
                if (text.Contains(":"))
                {

                    var prices = CardData.GetTcgPlayerPrices(cards.Select(x => x.ProductId).ToList());

                    var a = text.Split(":");
                    var set = a[0].Trim();
                    var ccard = a[1].Trim();
                    var orderedPrices = prices.OrderByDescending(p => p.midPrice).ToList();
                    if (ccard.Equals("**"))
                    {
                       orderedPrices = prices.Where(p => p.subTypeName == "Normal").OrderByDescending(p => p.midPrice).ToList(); 
                    }
                    


                    if (orderedPrices.Count() > 20)
                    {
                        orderedPrices.RemoveRange(20, orderedPrices.Count() - 20);
                    }

                    msg += "<i>Top " + orderedPrices.Count() + " prices of " + total + " results." + "</i>";

                    msg += "\r\n🔽" + "<b>" + CardData.TcgGroups.FirstOrDefault(g => g.groupId == cards[0].GroupId).name + "</b>:\r\n";

                    foreach (var orderedPrice in orderedPrices)
                    {
                        var card = cards.FirstOrDefault(c => c.ProductId == (int)orderedPrice.productId);

                        var priceNormal = string.Empty;
                        var priceFoil = string.Empty;

                        if (orderedPrice.subTypeName == "Normal")
                        {
                            priceNormal = orderedPrice.midPrice == null ? orderedPrice.marketPrice : orderedPrice.midPrice;
                        }
                        if (orderedPrice.subTypeName == "Foil")
                        {
                            priceFoil = orderedPrice.midPrice == null ? orderedPrice.marketPrice : orderedPrice.midPrice;
                        }

                        if (priceNormal != null && priceNormal != string.Empty && priceNormal != "null") priceNormal = "💵<b>$" + priceNormal + "</b>";
                        if (priceFoil != null && priceFoil != string.Empty && priceFoil != "null") priceFoil = "✨💵<b>$" + priceFoil + "</b>";

                        msg += "  ▶️" + card.Name + ": " + priceNormal + " " + priceFoil + " /tcgid_" + card.ProductId + "\r\n";

                    }

                    await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: message.MessageId);
                }

                else
                {

                    if (cards.Count > 15)
                    {
                        cards.RemoveRange(15, cards.Count() - 15);
                    }

                    var groupedCards = cards.OrderBy(z => z.Name).GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x);
                    msg += "<i>" + cards.Count() + " of " + total + " results." + "</i>";

                    var prices = CardData.GetTcgPlayerPrices(cards.Select(x => x.ProductId).ToList());


                    foreach (var groupedCard in groupedCards)
                    {
                        msg += "\r\n🔽" + "<b>" + groupedCard.Key + "</b>:\r\n";
                        foreach (var carditem in groupedCard.Value)
                        {
                            var priceNormal = string.Empty;
                            var priceFoil = string.Empty;
                            foreach (var cardPrice in prices)
                            {
                                if (cardPrice.productId == carditem.ProductId)
                                {
                                    if (cardPrice.subTypeName == "Normal")
                                    {
                                        priceNormal = cardPrice.midPrice == null ? cardPrice.marketPrice : cardPrice.midPrice;
                                    }
                                    if (cardPrice.subTypeName == "Foil")
                                    {
                                        priceFoil = cardPrice.midPrice == null ? cardPrice.marketPrice : cardPrice.midPrice;
                                    }
                                }
                            }

                            if (priceNormal != null && priceNormal != string.Empty && priceNormal != "null") priceNormal = "💵<b>$" + priceNormal + "</b>";
                            if (priceFoil != null && priceFoil != string.Empty && priceFoil != "null") priceFoil = "✨💵<b>$" + priceFoil + "</b>";


                            msg += "  ▶️" + CardData.TcgGroups.FirstOrDefault(g => g.groupId == carditem.GroupId).name + ": " + priceNormal + " " + priceFoil + " /tcgid_" + carditem.ProductId + "\r\n";
                        }
                    }

                    await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: message.MessageId);
                }
            }

            if (cards.Count == 1)
            {
                var command = new TcgSearchCommand();
                var mockMessage = message;
                mockMessage.Text = "/tcgid_" + cards[0].ProductId;
                await command.Execute(mockMessage, client, context);
            }

            if (cards.Count == 0)
            {
                msg = "<b>❌" + Lang.Res(lang).cardNotFoundByRequest + " \"" + text + "\".</b>\r\n" + Lang.Res(lang).tryAtTcgua + ".";
                await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: message.MessageId);
            }

        }

        public static List<Product> GetCardsFromDBBySet(string query, string targetSet, ApplicationDbContext context)
        {
            //var cardName = query.Query.Replace("tcg ", "");

            var set = CardData.TcgGroups.FirstOrDefault(g => g.name.ToLower().Equals(targetSet.ToLower()));
            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.abbreviation.ToLower().Equals(targetSet.ToLower()));
            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.name.ToLower().StartsWith(targetSet.ToLower()));
            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.abbreviation.ToLower().StartsWith(targetSet.ToLower()));
            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.name.ToLower().Contains(targetSet.ToLower()));
            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.abbreviation.ToLower().Contains(targetSet.ToLower()));

            if (set == null) return null;

            List<Product> results = new List<Product>();

            var cardsOfSet = context.Cards.Where(x => x.GroupId == set.groupId && x.ExtendedData.Contains("\"name\":\"Rarity\"")).ToList();
            var cards = new List<Product>();
            if (query.Equals("*") || query.Equals("**")) cards = cardsOfSet;
            else
            {
                var cardsAll = cardsOfSet.Where(x => x.Name.ToLower().Contains(query.ToLower())).ToList();

                var cards1 = cardsAll.Where(z => z.Name.Equals(query, StringComparison.InvariantCultureIgnoreCase)).ToList();
                cardsAll = cardsAll.Except(cards1).ToList();
                var cards2 = cardsAll.Where(z => z.Name.StartsWith(query, StringComparison.InvariantCultureIgnoreCase)).ToList();
                cardsAll = cardsAll.Except(cards2).ToList();

                cards = cards1;
                cards.AddRange(cards2);
                cards.AddRange(cardsAll);
            }

            if (cards.Count > 0)
            {
                foreach (var card in cards)
                {
                    results.Add(card);
                }
            }

            return results;
        }
    }
}