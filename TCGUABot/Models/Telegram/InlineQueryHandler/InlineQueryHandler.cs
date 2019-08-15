using System;
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
        public async void Execute(InlineQuery query, TelegramBotClient client)
        {
            bool ruLang = false;
            if (query.Query.Length > 2)
            {
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

                            results.Add(new InlineQueryResultArticle((++k).ToString(), card.name, new InputTextMessageContent("/c " + card.name+"("+set.code+")"))
                            {
                                HideUrl = true,
                                ThumbWidth = 99,
                                ThumbHeight = 138,
                                ThumbUrl = "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + muId + "&type=card",
                                Title = cardName + " (" + set.name + ")",
                                Description = card.type + " \r\n" + card.manaCost
                            });

                            if (results.Count > 49) break;
                        }
                    }
                    if (results.Count > 49) break;
                }

                await client.AnswerInlineQueryAsync(query.Id, results);
            }
        }
    }
}
