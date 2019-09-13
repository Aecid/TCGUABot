using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Data.Models;

namespace TCGUABot.Helpers
{
    public static class MythicSpoilerParsing
    {
        public static List<MythicSpoiler> GetNewSpoilers(ApplicationDbContext context)
        {
            HtmlWeb hw = new HtmlWeb();
            HtmlDocument doc = hw.Load("http://www.mythicspoiler.com/newspoilers.html");
            var result = new List<string>();
            foreach (HtmlNode htmlNode in doc.DocumentNode.SelectNodes("//img"))
            {
                if (htmlNode.GetAttributeValue("src", "").Contains("/cards/") && htmlNode.GetAttributeValue("src", "").Contains(".jpg"))
                {
                    result.Add("http://mythicspoiler.com/" + htmlNode.GetAttributeValue("src", ""));
                }

            }

            var oldCards = context.MythicSpoilers.ToList();
            var comparisonList = new List<string>();
            foreach (var oldCard in oldCards)
            {
                comparisonList.Add(oldCard.Url);
            }

            var toAdd = new List<MythicSpoiler>();
            foreach (var link in result)
            {
                if (!comparisonList.Contains(link))
                {
                    var spoiler = new MythicSpoiler();
                    spoiler.Url = link;
                    toAdd.Add(spoiler);
                }
            }

            if (toAdd.Count > 0)
            {
                context.MythicSpoilers.AddRange(toAdd);
                context.SaveChanges();
            }


            return toAdd;
        }
    }
}
