using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using TCGUABot.Models;

namespace TCGUABot.Pages.Decks
{
    public class DetailsModel : PageModel
    {
        private readonly TCGUABot.Data.ApplicationDbContext _context;

        public DetailsModel(TCGUABot.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public Deck Deck { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Deck = await _context.Decks.FirstOrDefaultAsync(m => m.Id == id);

            if (Deck == null)
            {
                return NotFound();
            }
            return Page();
        }

        public static dynamic ToDeck(string cards)
        {
            dynamic Deck = new ExpandoObject();
            Deck.MainDeck = new Dictionary<Card, int>();
            Deck.SideBoard = new Dictionary<Card, int>();

            if (cards != null)
            {
                var decklist = JsonConvert.DeserializeObject<dynamic>(cards);
                var Maindeck = new Dictionary<Card, int>();
                var Sideboard = new Dictionary<Card, int>();
                

                foreach (var card in decklist.MainDeck)
                {
                    Card foundCard = Helpers.CardSearch.GetCardByMultiverseId(int.Parse(card.Card.ToString()));
                    Maindeck.Add(foundCard, int.Parse(card.Count.ToString()));
                }

                foreach (var card in decklist.SideBoard)
                {
                    Card foundCard = Helpers.CardSearch.GetCardByMultiverseId(int.Parse(card.Card.ToString()));
                    Sideboard.Add(foundCard, int.Parse(card.Count.ToString()));
                }

                Deck.MainDeck = Maindeck;
                Deck.SideBoard = Sideboard;
            }

            return Deck;
        }

        public static string HtmlizeWithDividers(dynamic deck)
        {
            if (deck != null)
            {
                Dictionary<Card, int> MainDeck = deck.MainDeck;
                Dictionary<Card, int> Creatures = MainDeck.Where(c => c.Key.type.Contains("Creature", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Creatures).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Card, int> Planeswalkers = MainDeck.Where(c => c.Key.type.Contains("Planeswalker", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Planeswalkers).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Card, int> Spells = MainDeck.Where(c => c.Key.type.Contains("Instant", StringComparison.InvariantCultureIgnoreCase) || c.Key.type.Contains("Sorcery", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Spells).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Card, int> Artifacts = MainDeck.Where(c => c.Key.type.Contains("Artifact", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Artifacts).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Card, int> Enchantments = MainDeck.Where(c => c.Key.type.Contains("Enchantment", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Enchantments).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Card, int> Lands = MainDeck.Where(c => c.Key.type.Contains("Land", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Lands).ToDictionary(x => x.Key, x => x.Value);

                var all = new Dictionary<string, Dictionary<Card, int>>();
                all.Add("Creatures", Creatures);
                all.Add("Planeswalkers", Planeswalkers);
                all.Add("Spells", Spells);
                all.Add("Artifacts", Artifacts);
                all.Add("Enchantments", Enchantments);
                all.Add("Lands", Lands);


                var result = string.Empty;
                result += "<div class=\"deck-short\">";

                foreach (var item in all)
                {


                    if (item.Value.Count > 0)
                    {
                        result += "<p><b>" + item.Key + "</b><br/>";

                        foreach (var card in item.Value)
                        {
                            Card foundCard = Helpers.CardSearch.GetCardByMultiverseId(int.Parse(card.Key.multiverseId.ToString()));
                            result += card.Value + " " +
                                "<a target =\"_blank\"" +
                                "class=\"gathererTooltip\" " +
                                "data-image=\"https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + foundCard.multiverseId + "&type=card\" " +
                                "data-width=\"223px\"" +
                                "data-height=\"311px\"" +
                                "href=\"https://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=" + foundCard.multiverseId + "\"" +
                                ">" + foundCard.name + "</a><br/>";
                        }
                    }
                }


                result += "</p><p><b>Sideboard:</b><br/>";
                foreach (var card in deck.SideBoard)
                {
                    Card foundCard = Helpers.CardSearch.GetCardByMultiverseId(int.Parse(card.Key.multiverseId.ToString()));
                    result += card.Value + " " +
                        "<a target =\"_blank\"" +
                        "class=\"gathererTooltip\" " +
                        "data-image=\"https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + foundCard.multiverseId + "&type=card\" " +
                        "data-width=\"223px\"" +
                        "data-height=\"311px\"" +
                        "href=\"https://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=" + foundCard.multiverseId + "\"" +
                        ">" + foundCard.name + "</a><br/>";
                }
                result += "</p></div>";
                return result;
            }
            else
            {
                return "No cards";
            }
        }
    }
}
