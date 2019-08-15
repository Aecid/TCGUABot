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
            var userId = "548ba8ce-d90a-4f33-834a-bc2a78372df6";
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
            string zz = @"4 Fiery Islet 4 Light Up the Stage 4 Crash Through 4 Soul-Scar Mage 4 Bedlam Reveler 4 Monastery Swiftspear 2 Blistercoil Weird 4 Faithless Looting 4 Manamorphose 4 Lava Spike 4 Lava Dart 14 Mountain 4 Lightning Bolt  Sideboard 1 Shenanigans 3 Abrade 1 Dismember 4 Surgical Extraction 2 Flame Slash 3 Dragon's Claw 1 Tormod's Crypt";

            

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
            context.Decks.Add(dbdeck);
            context.SaveChanges();
            
            return deckId;
        }
    }
}
