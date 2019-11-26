﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InlineQueryResults;

namespace TCGUABot.Models.InlineQueryHandler
{
    public class InlineQueryHandler
    {
        public async Task Execute(InlineQuery query, TelegramBotClient client)
        {
            if (query.Query.StartsWith("set"))
            {

            }
            else
            {
                if (query.Query.Length > 3)
                {
                    var results = new List<InlineQueryResultArticle>();
                    if (Regex.IsMatch(query.Query, @"\p{IsCyrillic}")) results = GetCards(query);
                    else results = GetCardsFromTcg(query);

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
                            results.Add(new InlineQueryResultArticle((++k).ToString(), card.name, new InputTextMessageContent("/tcgid "+card.tcgplayerProductId))
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

        public List<InlineQueryResultArticle> GetCardsFromTcg (InlineQuery query)
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
    }
}
