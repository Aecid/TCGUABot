using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace TCGUABot.Models.InlineQueryHandler
{
    public class InlineQueryHandler
    {
        public async Task Execute(InlineQuery query, TelegramBotClient client, ApplicationDbContext context)
        {
            if (query.Query.Contains(":"))
            {
                var a = query.Query.Split(":");
                var set = a[0].Trim();
                var card = a[1].Trim();

                if (set.Length > 2 && (card.Length > 2 || card.Equals("*")))
                {
                    var results = GetCardsFromDBBySet(card, set, context);

                    try
                    {
                        if (results.Count > 0)
                        {
                            await client.AnswerInlineQueryAsync(query.Id, results);
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
                if (query.Query.Length > 2)
                {
                    var results = new List<InlineQueryResultArticle>();
                    if (Regex.IsMatch(query.Query, @"\p{IsCyrillic}")) results = GetCards(query);
                    //else results = GetCardsFromTcg(query);
                    else results = GetCardsFromDB(query, context);

                    try
                    {
                        if (results.Count > 0)
                        {
                            await client.AnswerInlineQueryAsync(query.Id, results);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message + "|" + e?.InnerException?.Message);
                    }
                }
            }
        }

        public List<InlineQueryResultArticle> GetCards(InlineQuery query)
        {
            bool ruLang = false;

            List<InlineQueryResultArticle> results = new List<InlineQueryResultArticle>();
            var k = 0;
            foreach (var set in CardData.Instance.Sets)
            {
                List<Card> cards = new List<Card>();
                if (Regex.IsMatch(query.Query, @"\p{IsCyrillic}"))
                {
                    ruLang = true;
                    cards = set.cards.Where(c => c.ruName.ToLower().Contains(query.Query.ToLower())).ToList();
                }
                else
                {
                    ruLang = false;
                    cards = set.cards.Where(c => c.name.ToLower().Contains(query.Query.ToLower())).ToList();
                }

                if (cards.Count > 0)
                {
                    foreach (var card in cards)
                    {
                        var muId = ruLang ? card.ruMultiverseId : card.multiverseId;
                        var cardName = ruLang ? card.ruName : card.name;

                        if (card.tcgplayerProductId > 0)
                        {
                            results.Add(new InlineQueryResultArticle((++k).ToString(), card.name, new InputTextMessageContent("/tcgid " + card.tcgplayerProductId))
                            {
                                HideUrl = true,
                                ThumbWidth = 99,
                                ThumbHeight = 138,
                                ThumbUrl = "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + muId + "&type=card",
                                Title = cardName + " (" + set.name + ")",
                                Description = card.type + " \r\n" + card.manaCost
                            });
                        }
                        else
                        {
                            results.Add(new InlineQueryResultArticle((++k).ToString(), card.name, new InputTextMessageContent("/c " + card.name + "(" + set.code + ")"))
                            {
                                HideUrl = true,
                                ThumbWidth = 99,
                                ThumbHeight = 138,
                                ThumbUrl = "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + muId + "&type=card",
                                Title = cardName + " (" + set.name + ")",
                                Description = card.type + " \r\n" + card.manaCost
                            });
                        }

                        if (results.Count > 49) break;
                    }
                }
                if (results.Count > 49) break;
            }

            return results;
        }

        public List<InlineQueryResultArticle> GetCardsFromTcg(InlineQuery query)
        {
            //var cardName = query.Query.Replace("tcg ", "");

            List<InlineQueryResultArticle> results = new List<InlineQueryResultArticle>();

            var cards = CardData.TcgSearchByName(query.Query);

            if (cards.Count > 0)
            {
                var k = 0;
                foreach (var card in cards)
                {
                    results.Add(new InlineQueryResultArticle((++k).ToString(), card.name, new InputTextMessageContent("/tcgid " + card.productId))
                    {
                        HideUrl = true,
                        ThumbWidth = 99,
                        ThumbHeight = 138,
                        ThumbUrl = card.imageUrl,
                        Title = card.name,
                        Description = CardData.TcgGroups.FirstOrDefault(z => z.groupId == card.groupId).name,
                    });
                }
            }

            return results;
        }

        public List<InlineQueryResultArticle> GetCardsFromDB(InlineQuery query, ApplicationDbContext context)
        {
            //var cardName = query.Query.Replace("tcg ", "");

            List<InlineQueryResultArticle> results = new List<InlineQueryResultArticle>();

            var cardsAll = context.Cards.Where(x => EF.Functions.ILike(x.Name, $"%{query.Query}%")).ToList();

            var cards1 = cardsAll.Where(z => z.Name.Equals(query.Query, StringComparison.InvariantCultureIgnoreCase)).ToList();
            cardsAll = cardsAll.Except(cards1).ToList();
            var cards2 = cardsAll.Where(z => z.Name.StartsWith(query.Query, StringComparison.InvariantCultureIgnoreCase)).ToList();
            cardsAll = cardsAll.Except(cards2).ToList();

            var cards = cards1;
            cards.AddRange(cards2);
            cards.AddRange(cardsAll);

            if (cards.Count > 50)
            {
                cards.RemoveRange(50, cards.Count - 50);
            }

            if (cards.Count > 0)
            {
                var k = 0;
                foreach (var card in cards)
                {
                    var currentSet = CardData.TcgGroups.FirstOrDefault(z => z.groupId == card.GroupId);
                    results.Add(new InlineQueryResultArticle((++k).ToString(), card.Name, new InputTextMessageContent("/tcgid " + card.ProductId))
                    {
                        HideUrl = true,
                        ThumbWidth = 99,
                        ThumbHeight = 138,
                        ThumbUrl = card.ImageUrl,
                        Title = card.Name,
                        Description = currentSet.name + " ("+currentSet.abbreviation+")",
                    });
                }
            }

            return results;
        }

        public List<InlineQueryResultArticle> GetCardsFromDBBySet(string query, string targetSet, ApplicationDbContext context)
        {
            //var cardName = query.Query.Replace("tcg ", "");

            var set = CardData.TcgGroups.FirstOrDefault(g => g.name.ToLower().Equals(targetSet.ToLower()));
            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.abbreviation.ToLower().Equals(targetSet.ToLower()));
            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.name.ToLower().StartsWith(targetSet.ToLower()));
            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.abbreviation.ToLower().StartsWith(targetSet.ToLower()));
            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.name.ToLower().Contains(targetSet.ToLower()));
            if (set == null) set = CardData.TcgGroups.FirstOrDefault(g => g.abbreviation.ToLower().Contains(targetSet.ToLower()));

            if (set == null) return null;

            List<InlineQueryResultArticle> results = new List<InlineQueryResultArticle>();

            var cardsOfSet = context.Cards.Where(x => x.GroupId == set.groupId).ToList();
            var cards = new List<Product>();
            if (query.Equals("*")) cards = cardsOfSet;
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

            if (cards.Count > 50)
            {
                cards.RemoveRange(50, cards.Count - 50);
            }

            if (cards.Count > 0)
            {
                var k = 0;
                foreach (var card in cards)
                {
                    results.Add(new InlineQueryResultArticle((++k).ToString(), card.Name, new InputTextMessageContent("/tcgid " + card.ProductId))
                    {
                        HideUrl = true,
                        ThumbWidth = 99,
                        ThumbHeight = 138,
                        ThumbUrl = card.ImageUrl,
                        Title = card.Name,
                        Description = CardData.TcgGroups.FirstOrDefault(z => z.groupId == card.GroupId).name,
                    });
                }
            }

            return results;
        }
    }
}
