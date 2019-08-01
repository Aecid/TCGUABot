using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
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

            var NonNullSets = cards.Sets.Where(s => s.Value.cards.Any(c => c.multiverseId != 0)).ToList();

            for (int i = 0; i < 60; i++)
            {
                Set set = NonNullSets.ElementAt(random.Next(NonNullSets.Count())).Value;
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

        // POST: api/Decklist
        [HttpPost]
        public string Post([FromBody] DeckArenaImport deck)
        {
            dynamic deckList = new ExpandoObject();
            deckList.MainDeck = new List<ExpandoObject>();
            deckList.SideBoard = new List<ExpandoObject>();
            foreach (var importCard in deck.MainDeck)
            {
                var set = importCard.set.Replace("(", "").Replace(")", "").Replace("DAR", "DOM");
                var card = CardData.Instance.Sets[set].cards.FirstOrDefault(c => c.number == importCard.collectorNumber.ToString());
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
                var card = CardData.Instance.Sets[set].cards.FirstOrDefault(c => c.number == importCard.collectorNumber.ToString());
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
