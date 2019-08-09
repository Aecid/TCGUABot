using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCGUABot.Models;

namespace TCGUABot.Controllers
{
    public class DecklistController : Controller
    {
        // GET: api/Decklist
        [HttpGet]
        public ActionResult RandomDeck()//IEnumerable<string> Get()
        {
            var cards = CardData.Instance;
            var random = new Random();
            //var obj = cards.Cards.ElementAt(random.Next(cards.Cards.Count));
            Dictionary<string, string> urls = new Dictionary<string, string>();
            List<Card> decklist = new List<Card>();

            var NonNullSets = cards.Sets.Where(s => s.cards.Any(c => c.multiverseId != 0)).ToList();

            for (int i = 0; i < 60; i++)
            {
                Set set = NonNullSets.ElementAt(random.Next(NonNullSets.Count()));
                Card obj = set.cards.Where(c => c.multiverseId != 0).ElementAt(random.Next(set.cards.Where(c => c.multiverseId != 0).Count()));
                //urls.Add(obj.name, "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + obj.multiverseId + "&type=card");
                decklist.Add(obj);
                if (decklist.Count == 60) break;
                if (random.Next(0, 2) == 1) decklist.Add(obj);
                if (decklist.Count == 60) break;
                if (random.Next(0, 2) == 1) decklist.Add(obj);
                if (decklist.Count == 60) break;
                if (random.Next(0, 2) == 1) decklist.Add(obj);
                if (decklist.Count == 60) break;
            }

            ViewBag.Urls = urls;
            ViewBag.Decklist = decklist;

            var sortedDecklist =
                decklist
                .GroupBy(x => x)
                .Select(x => new
                {
                    Card = x.Key,
                    Quantity = x.Count(),
                })
                .OrderByDescending(x => x.Quantity)
                .ToList();

            var expandoDecklist = new List<ExpandoObject>();
            foreach (var item in sortedDecklist)
            {
                dynamic z = new ExpandoObject();
                z.Card = item.Card;
                z.Count = item.Quantity;
                expandoDecklist.Add(z);
            }

            ViewBag.SortedDecklist = expandoDecklist;


            return View();
        }

        // GET: api/Decklist/5
        [HttpGet("/decks/{deckId}", Name = "Get_Deck")]
        public ActionResult GetDeck(string deckId)
        {
            var deck = JsonConvert.DeserializeObject<ExpandoObject>(System.IO.File.ReadAllText(deckId + ".json"));
            ViewBag.Deck = deck;
            return View();
        }

        [HttpGet("/test2", Name = "Test2")]
        public string Test()
        {
            string zz = @"/import 4 Arboreal Grazer (WAR) 149
4 Elvish Rejuvenator (M19) 180
4 Hydroid Krasis (RNA) 183
4 Teferi, Time Raveler (WAR) 221
2 Forest (XLN) 277
2 Island (XLN) 265
1 Plains (XLN) 261
1 Azorius Guildgate (RNA) 243
1 Blast Zone (WAR) 244
1 Blossoming Sands (M20) 243
2 Breeding Pool (RNA) 246
1 Field of Ruin (XLN) 254
4 Field of the Dead (M20) 247
2 Hallowed Fountain (RNA) 251
1 Hinterland Harbor (DAR) 240
1 Selesnya Guildgate (GRN) 256
1 Simic Guildgate (RNA) 257
1 Sunpetal Grove (XLN) 257
2 Temple Garden (GRN) 258
1 Temple of Malady (M20) 254
2 Temple of Mystery (M20) 255
1 Thornwood Falls (M20) 258
1 Tranquil Cove (M20) 259
4 Growth Spiral (RNA) 178
4 Circuitous Route (GRN) 125
2 Grow from the Ashes (DAR) 164
4 Scapeshift (M19) 201
2 Time Wipe (WAR) 223

2 Deputy of Detention (RNA) 165
2 Baffling End (RIX) 1
2 Ixalan's Binding (XLN) 17
2 Dovin's Veto (WAR) 193
3 Root Snare (M19) 199
3 Veil of Summer (M20) 198
1 Ashiok, Dream Render (WAR) 228";


            var text = zz.Replace("/import ", "");
            var deck = new DeckArenaImport();
            deck.MainDeck = new List<ArenaCard>();
            deck.SideBoard = new List<ArenaCard>();
            bool side = false;
            foreach (var myString in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.None))
            {
                ArenaCard card = new ArenaCard();

                if (myString.Trim().Length > 1)
                {
                    var cardData = myString.Split(" ");
                    Regex exp = new Regex(@"(\d+)\s+(.*)\s+(\(.+\))\s+(\d+)");
                    var matches = exp.Matches(myString);

                    int.TryParse(matches[0].Groups[1].Value, out card.count);
                    card.name = matches[0].Groups[2].Value;
                    card.set = matches[0].Groups[3].Value;
                    int.TryParse(matches[0].Groups[4].Value, out card.collectorNumber);

                    if (side) deck.SideBoard.Add(card);
                    else deck.MainDeck.Add(card);
                }

                else
                {
                    side = true;
                }
            }

            var controller = new TCGUABot.Controllers.DecklistController();
            var id = controller.Import(deck);

            return id;
        }

        [HttpGet("/card", Name="TestCard")]
        public string GetCard(string query)
        {
            //https://api.scryfall.com/cards/multiverse/464166
            dynamic card = JsonConvert.DeserializeObject<dynamic>(CardData.ApiCall("https://api.scryfall.com/cards/multiverse/" + 464166));
            var prices = card.prices;
            return card.prices.usd;
        }

        // POST: api/Decklist
        [HttpPost]
        public string Import([FromBody] DeckArenaImport deck)
        {
            var z = deck.MainDeck.GroupBy(x => x.name);


            var sortedDeck = new DeckArenaImport();
            sortedDeck.MainDeck = new List<ArenaCard>();
            sortedDeck.SideBoard = new List<ArenaCard>();

            foreach (var card in deck.MainDeck)
            {
                foreach (var compareCard in deck.MainDeck)
                {
                    if (card != compareCard)
                    {
                        if (card.name.Equals(compareCard.name) && !card.set.Equals(compareCard.set))
                        {
                            card.count += compareCard.count;
                        }
                    }
                }

                if (!sortedDeck.MainDeck.Any(c => c.name.Equals(card.name)))
                {
                    sortedDeck.MainDeck.Add(card);
                }
            }

            foreach (var card in deck.SideBoard)
            {
                foreach (var compareCard in deck.SideBoard)
                {
                    if (card != compareCard)
                    {
                        if (card.name.Equals(compareCard.name) && !card.set.Equals(compareCard.set))
                        {
                            card.count += compareCard.count;
                        }
                    }
                }

                if (!sortedDeck.SideBoard.Any(c => c.name.Equals(card.name)))
                {
                    sortedDeck.SideBoard.Add(card);
                }
            }




            dynamic deckList = new ExpandoObject();
            deckList.MainDeck = new List<ExpandoObject>();
            deckList.SideBoard = new List<ExpandoObject>();
            foreach (var importCard in sortedDeck.MainDeck)
            {
                var set = importCard.set.Replace("(", "").Replace(")", "").Replace("DAR", "DOM");
                var card = CardData.Instance.Sets.FirstOrDefault(s => s.name.Equals(set)).cards.FirstOrDefault(c => c.number == importCard.collectorNumber.ToString());
                if (card.name.Equals(importCard.name))
                {
                    dynamic obj = new ExpandoObject();
                    obj.Card = card;
                    obj.Count = importCard.count;
                    deckList.MainDeck.Add(obj);
                }
            }
            foreach (var importCard in deck.SideBoard)
            {
                var set = importCard.set.Replace("(", "").Replace(")", "");
                var card = CardData.Instance.Sets.FirstOrDefault(s => s.name.Equals(set)).cards.FirstOrDefault(c => c.number == importCard.collectorNumber.ToString());
                if (card.name.Equals(importCard.name))
                {
                    dynamic obj = new ExpandoObject();
                    obj.Card = card;
                    obj.Count = importCard.count;
                    deckList.SideBoard.Add(obj);
                }
            }

            var deckId = Guid.NewGuid().ToString();
            try
            {
                System.IO.File.WriteAllText(deckId + ".json", JsonConvert.SerializeObject(deckList));
            }
            catch
            {
                return "NULL";
            }

            return deckId;
        }
    }
}
