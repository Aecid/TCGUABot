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

            if (deck.MainDeck.Count == 0)
            {
                throw new InvalidOperationException("Deck is empty or in wrong format.");
            }

            var deckId = Guid.NewGuid().ToString();

            var dbdeck = new Deck();

            dbdeck.ApplicationUser = deck.Owner;
            dbdeck.UserId = deck.Owner.Id;
            dbdeck.Name = deck.MainDeck[0].name;
            dbdeck.Id = deckId;
            dbdeck.Cards = deck.ToString();
            dbdeck.CreationDate = DateTime.UtcNow;
            context.Decks.Add(dbdeck);
            context.SaveChanges();
            
            return deckId;
        }
    }
}
