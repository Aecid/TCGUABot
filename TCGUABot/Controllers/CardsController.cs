using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using TCGUABot.Models;
using TCGUABot.Models.Commands;
using Telegram.Bot.Types;
using Z.EntityFramework.Plus;

namespace TCGUABot.Controllers
{
    public class CardsController : Controller
    {
        ApplicationDbContext context { get; set; }
        public CardsController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [Produces("application/json")]
        [HttpGet]
        public IActionResult SearchByName([FromQuery]string q)
        {
            if (string.IsNullOrEmpty(q)) return BadRequest();
            if (q.Length < 3) return BadRequest();

            try
            {
                var results = new List<dynamic>();
                bool ruLang = false;
                foreach (var set in CardData.Instance.Sets)
                {
                    List<Card> cards = new List<Card>();
                    if (Regex.IsMatch(q, @"\p{IsCyrillic}"))
                    {
                        ruLang = true;
                        cards = set.cards.Where(c => c.ruName.Contains(q, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    }
                    else
                    {
                        ruLang = false;
                        cards = set.cards.Where(c => c.name.Contains(q, StringComparison.InvariantCultureIgnoreCase)).ToList();
                    }

                    if (cards.Count > 0)
                    {
                        foreach (var card in cards)
                        {
                            var muId = ruLang ? card.ruMultiverseId : card.multiverseId;
                            var cardName = ruLang ? card.ruName : card.name;
                            var cardType = ruLang ? card.foreignData.FirstOrDefault(f => f.language == "Russian").type : card.type;


                            dynamic cardData = new ExpandoObject();
                            cardData.url = "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + muId + "&type=card";
                            cardData.name = cardName;
                            cardData.type = cardType ?? card.type;
                            if (card.manaCost != null) cardData.manacost = ImportDeck.ReplaceManaSymbols(card.manaCost);
                            else cardData.manacost = "";
                            cardData.set = set.code;
                            cardData.setName = set.name;

                            results.Add(cardData);

                            if (results.Count > 49) break;
                        }
                    }
                    else
                    {
                        //return NotFound();
                    }

                    if (results.Count > 49) break;
                }

                var sortedResults = results.OrderBy(x => ((IDictionary<string, object>)x)["name"].ToString()).ThenBy(x => ((IDictionary<string, object>)x)["set"].ToString()).ToList();
                if (sortedResults.Count > 0) return Ok(sortedResults);
                else return BadRequest();
            }
            catch (Exception e)
            {
                return BadRequest(e);
            }
        }

        [Produces("application/json")]
        [HttpGet]
        public IActionResult SearchByExactName([FromQuery]string q)
        {
            dynamic cardData = new ExpandoObject();
            string text = string.Empty;
            string set = string.Empty;
            if (q.Contains("(") && q.Contains(")"))
            {
                var match = Regex.Match(q, @"(.*)\((.*)\)");
                text = match.Groups[1].Value;
                set = match.Groups[2].Value;
            }
            else
            {
                text = q;
            }
            var msg = string.Empty;
            Card card;
            if (!string.IsNullOrEmpty(set))
            {
                card = Helpers.CardSearch.GetCardByName(text.Trim(), set);
            }
            else
            {
                card = Helpers.CardSearch.GetCardByName(text.Trim(), true);
            }

            if (card == null) return NotFound(text);

            set = string.IsNullOrEmpty(set) ? card.Set : set;

            cardData.set = set;
            cardData.setName = CardData.Instance.Sets.FirstOrDefault(s => s.code == set).name;
            cardData.query = text;
            cardData.nameEn = card.name;
            if (card.foreignData.Any(c => c.language.Equals("Russian"))) cardData.nameRu = card.ruName;

            try
            {
                var prices = CardData.GetTcgPlayerPrices(card.tcgplayerProductId);
                cardData.prices = prices;
            }
            catch
            {
                cardData.prices = new List<string>();
            }

            var urls = new List<string>();


            if (card.names != null && card.names.Count > 0)
            {

                var addname = "";
                var addruname = "";
                var k = 0;
                foreach (var name in card.names)
                {
                    var addcard = Helpers.CardSearch.GetCardByName(name, set);
                    if (k > 0) addname += " | ";
                    addname += addcard.name;

                    if (!string.IsNullOrEmpty(addcard.ruName))
                    {
                        if (k > 0) addruname += " | ";
                        addruname += addcard.ruName;

                    }

                    if (addcard.multiverseId > 0)
                        urls.Add("https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + addcard.multiverseId + "&type=card");
                    else
                        urls.Add(CardData.GetTcgPlayerImage(card.tcgplayerProductId));

                    k++;
                }

                if (!string.IsNullOrEmpty(addname)) cardData.nameEn = addname;
                if (!string.IsNullOrEmpty(addruname)) cardData.nameRu = addruname;
            }
            else
            {
                if (card.multiverseId > 0)
                    urls.Add("https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card");
                else
                    urls.Add(CardData.GetTcgPlayerImage(card.tcgplayerProductId));
            }

            cardData.urls = urls;


            return Ok(cardData);
        }
    }
}
