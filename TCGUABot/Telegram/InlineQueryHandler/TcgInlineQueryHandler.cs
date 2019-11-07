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
    public class TcgInlineQueryHandler
    {
        public async Task Execute(InlineQuery query, TelegramBotClient client)
        {
            if (query.Query.StartsWith("set"))
            {

            }
            else
            {
                if (query.Query.Length > 2)
                {
                    var results = GetCards(query);
                    try
                    {
                        await client.AnswerInlineQueryAsync(query.Id, results);
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
