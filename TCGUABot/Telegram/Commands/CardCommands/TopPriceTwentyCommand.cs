//using Newtonsoft.Json;
//using System;
//using System.Collections.Generic;
//using System.Dynamic;
//using System.IO;
//using System.Linq;
//using System.Net;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using TCGUABot.Data;
//using TCGUABot.Data.Migrations;
//using TCGUABot.Resources;
//using Telegram.Bot;
//using Telegram.Bot.Types;
//using Telegram.Bot.Types.InlineQueryResults;
//using Telegram.Bot.Types.InputFiles;
//using Telegram.Bot.Types.ReplyMarkups;

//namespace TCGUABot.Models.Commands
//{
//    //TODO switch from CardCommand to this
//    public class TopPriceTwentyCommand : Command
//    {
//        public override string Name => "/top20";

//        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
//        {
//            var tUser = message.From;
//            Helpers.TelegramUtil.AddUser(tUser, context);

//            var chatId = message.Chat.Id;
//            try
//            {
//                await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadPhoto);
//            }
//            catch
//            {

//            }

//            string text = string.Empty;
//            string setName = string.Empty;
//            var originalMessage = message.Text.Substring(message.Text.IndexOf(Name)).Trim().Replace("@tcgua_bot", "");
//            text = originalMessage.Replace(Name, "").Trim();

//            var set = string.Empty;
//            if (text.Length > 2) set = text;

//            var lang = context.TelegramChats.FirstOrDefault(f => f.Id == chatId)?.Language;
//            lang = lang == null ? "ru" : lang;
//            var msg = string.Empty;


//            var results = new List<Product>();

//            if (set.Length > 2)
//            {
//                results = GetAllCardsFromDBBySet(set, context);
//            }
//            else
//            {
//                results = GetTop100CardsByPrice(context);
//            }

//            cards.AddRange(cardsFull);
//            cards.AddRange(cardsStartsWith);
//            cards.AddRange(cardsContains);

//            var total = cards.Count();

//            if (cards.Count() == 0) msg = "<b>❌" + Lang.Res(lang).cardNotFoundByRequest + " \"" + text + "\".</b>\r\n" + Lang.Res(lang).tryAtTcgua + ".";
//            if (cards.Count() > 15)
//            {
//                cards.RemoveRange(15, cards.Count() - 15);
//            }
//            if (cards.Count > 1)
//            {
//                var groupedCards = cards.OrderBy(z=>z.Name).GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x);
//                msg += "<i>" + cards.Count() + " of " + total + " results." + "</i>";

//                var prices = CardData.GetTcgPlayerPrices(cards.Select(x => x.ProductId).ToList());
//                foreach (var groupedCard in groupedCards)
//                {
//                    msg += "\r\n🔽" + "<b>" + groupedCard.Key + "</b>:\r\n";
//                    foreach (var carditem in groupedCard.Value)
//                    {
//                        var priceNormal = string.Empty;
//                        var priceFoil = string.Empty;
//                        foreach (var cardPrice in prices)
//                        {
//                            if (cardPrice.productId == carditem.ProductId)
//                            {
//                                if (cardPrice.subTypeName == "Normal")
//                                {
//                                    priceNormal = cardPrice.midPrice == null ? cardPrice.marketPrice : cardPrice.midPrice;
//                                }
//                                if (cardPrice.subTypeName == "Foil")
//                                {
//                                    priceFoil = cardPrice.midPrice == null ? cardPrice.marketPrice : cardPrice.midPrice;
//                                }
//                            }
//                        }

//                        if (priceNormal != null && priceNormal != string.Empty && priceNormal != "null") priceNormal = "💵<b>$" + priceNormal + "</b>";
//                        if (priceFoil != null && priceFoil != string.Empty && priceFoil != "null") priceFoil = "✨💵<b>$" + priceFoil + "</b>";


//                        msg += "  ▶️" + CardData.TcgGroups.FirstOrDefault(g => g.groupId == carditem.GroupId).name + ": " + priceNormal + " " + priceFoil + " /tcgid_" + carditem.ProductId+"\r\n";
//                    }
//                }

//                await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId:message.MessageId);
//            }            

//            if (cards.Count == 1)
//            {
//                var command = new TcgSearchCommand();
//                var mockMessage = message;
//                mockMessage.Text = "/tcgid_" + cards[0].ProductId;
//                await command.Execute(mockMessage, client, context);
//            }

//        }

//        private List<Product> GetTop100CardsByPrice(ApplicationDbContext context)
//        {
//            throw new NotImplementedException();
//        }

//        private List<Product> GetAllCardsFromDBBySet(string targetSet, ApplicationDbContext context)
//        {
//            var result = new List<Product>();

//            var set = CardData.TcgGroups.FirstOrDefault(g => g.name.ToLower().Equals(targetSet.ToLower()));
//            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.abbreviation.ToLower().Equals(targetSet.ToLower()));
//            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.name.ToLower().StartsWith(targetSet.ToLower()));
//            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.abbreviation.ToLower().StartsWith(targetSet.ToLower()));
//            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.name.ToLower().Contains(targetSet.ToLower()));
//            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.abbreviation.ToLower().Contains(targetSet.ToLower()));

//            if (set == null) return null;

//            var cardsOfSet = context.Cards.Where(x => x.GroupId == set.groupId).ToList();

//            List<int> ids = new List<int>();

//            foreach (var card in cardsOfSet)
//            {
//                ids.Add(card.ProductId);
//            }

//            var res = CardData.GetTcgPlayerPrices(ids).OrderBy(x => x.midPrice);


//        }
//    }
//}