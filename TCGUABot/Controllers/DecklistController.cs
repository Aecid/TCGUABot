using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using TCGUABot.Models;
using Z.EntityFramework.Plus;

namespace TCGUABot.Controllers
{
    public class DecklistController : Controller
    {
        ApplicationDbContext context { get; set; }
        public DecklistController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public string Clear()
        {
            var userId = "d34f08f5-9daa-46d6-a87c-cc3a6fda538a";
            var user = context.Users.FirstOrDefault(u => u.Id == userId);

            context.Decks.Where(x => x.ApplicationUser.Id == userId).Delete();

            return "Should be done";
        }

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
        [HttpGet("/deckz/{deckId}", Name = "Get_Deck")]
        public ActionResult GetDeck(string deckId)
        {
            var deck = context.Decks.FirstOrDefault(d => d.Id == deckId);
            ViewBag.Title = deck.Name;
            //var deck = JsonConvert.DeserializeObject<ExpandoObject>(System.IO.File.ReadAllText(deckId + ".json"));
            ViewBag.Deck = JsonConvert.DeserializeObject<ExpandoObject>(deck.Cards);
            return View();
        }

        [HttpGet("/deckt2/{deckId}", Name = "Get_Deck_t2")]
        public ActionResult GetDeckType2(string deckId)
        {
            var deck = context.Decks.FirstOrDefault(d => d.Id == deckId);
            ViewBag.Title = deck.Name;
            //var deck = JsonConvert.DeserializeObject<ExpandoObject>(System.IO.File.ReadAllText(deckId + ".json"));
            ViewBag.Deck = JsonConvert.DeserializeObject<ExpandoObject>(deck.Cards);
            return View();
        }

        [HttpGet("/deck/{deckId}", Name = "Get_Deck_Text")]
        public ActionResult GetDeckText(string deckId)
        {
            var deck = context.Decks.FirstOrDefault(d => d.Id == deckId);
            ViewBag.Title = deck.Name;
            //var deck = JsonConvert.DeserializeObject<ExpandoObject>(System.IO.File.ReadAllText(deckId + ".json"));
            ViewBag.Deck = JsonConvert.DeserializeObject<ExpandoObject>(deck.Cards);
            return View();
        }

        [HttpGet("/test", Name = "Test2")]
        public string Test()
        {
            string zz = @"3 Arboreal Grazer (WAR) 149
4 Growth Spiral (RNA) 178
4 Hydroid Krasis (RNA) 183
4 Elvish Rejuvenator (M19) 180
2 Prison Realm (WAR) 26
2 Grow from the Ashes (DAR) 164
4 Teferi, Time Raveler (WAR) 221
4 Circuitous Route (GRN) 125
4 Scapeshift (M19) 201
1 Plains (XLN) 261
2 Island (XLN) 265
2 Forest (XLN) 277
1 Azorius Guildgate (RNA) 243
2 Hallowed Fountain (RNA) 251
1 Tranquil Cove (M20) 259
1 Temple of Malady (M20) 254
1 Blossoming Sands (M20) 243
1 Selesnya Guildgate (GRN) 256
1 Sunpetal Grove (XLN) 257
2 Temple Garden (GRN) 258
3 Breeding Pool (RNA) 246
1 Hinterland Harbor (DAR) 240
1 Simic Guildgate (RNA) 257
2 Temple of Mystery (M20) 255
1 Thornwood Falls (M20) 258
1 Blast Zone (WAR) 244
1 Field of Ruin (XLN) 254
4 Field of the Dead (M20) 247

3 Veil of Summer (M20) 198
3 Aether Gust (M20) 42
3 Dovin's Veto (WAR) 193
3 Deputy of Detention (RNA) 165
2 Knight of Autumn (GRN) 183
1 Ixalan's Binding (XLN) 17";

            

            var text = zz.Replace("/import ", "");

            var deck = ImportDeck.StringToDeck(text, context.Users.FirstOrDefault(u => u.Id == "4d72cde2-2f9d-4463-b0db-fac43c552544"));

            var id = Import(deck);

            return id;
        }

        [HttpGet("/card", Name = "TestCard")]
        public string GetCard(string query)
        {
            //https://api.scryfall.com/cards/multiverse/464166
            dynamic card = JsonConvert.DeserializeObject<dynamic>(CardData.ApiCall("https://api.scryfall.com/cards/multiverse/" + 464166));
            var prices = card.prices;
            return card.prices.usd;
        }

        [HttpGet("/weirdCards", Name = "weirdCards")]
        public string GetCard()
        {
            string z = string.Empty;
            foreach (var set in CardData.Instance.Sets)
            {
                foreach (var card in set.cards)
                {
                    var regexItem = new Regex(@"[0-9\(\)]");

                    if (regexItem.IsMatch(card.name))
                        z += card.name+" "+ set.name +"\r\n";
                }
            }

            return z;
        }

        [HttpPost]
        public string HtmlizeStringDeck([FromBody] string deck)
        {
            var result = ImportDeck.HtmlizeString(deck);
            return result;
        }

        // POST: /Decklist
        [HttpPost]
        public string Import([FromBody] ImportDeck deck)
        {


            var deckId = Guid.NewGuid().ToString();

            var dbdeck = new Deck();

            dbdeck.ApplicationUser = deck.Owner;
            dbdeck.UserId = deck.Owner.Id;
            dbdeck.Name = "TestDeck";
            dbdeck.Id = deckId;
            dbdeck.Cards = deck.ToString();
            dbdeck.CreationDate = DateTime.UtcNow;
            context.Decks.Add(dbdeck);
            context.SaveChanges();
            
            return deckId;
        }
    }
}
