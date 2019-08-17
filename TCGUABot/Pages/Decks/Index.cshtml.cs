using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using TCGUABot.Models;

namespace TCGUABot.Pages.Decks
{
    public class IndexModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public IndexModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IList<Deck> Deck { get; set; }

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                Deck = await _context.Decks.Where(d => d.ApplicationUser == user).ToListAsync();
            }
            else
            {
                var user = _context.Users.FirstOrDefault(u => u.Id == "d34f08f5-9daa-46d6-a87c-cc3a6fda538a");

                Deck = await _context.Decks.Where(d => d.ApplicationUser == user).ToListAsync();
            }
        }

        public static string ToShortDeck(string cards)
        {
            if (cards != null)
            {
                var decklist = ImportDeck.StringToDeck(cards, null);
                var result = string.Empty;
                result += "<div class=\"deck-short\"";
                result += "<p><b>Maindeck:</b><br/>";
                foreach (var card in decklist.MainDeck)
                {
                    result += card.count + "x " +
                        "<a " +
                        "class=\"gathererTooltip\" " +
                        "data-image=\"https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card\" " +
                        "data-width=\"223px\"" +
                        "data-height=\"311px\"" +
                        "href=\"https://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=" + card.multiverseId + "\"" +
                        ">" + card.name + "</a><br/>";
                }
                result += "</p><p><b>Sideboard:</b><br/>";
                foreach (var card in decklist.SideBoard)
                {
                    result += card.count + "x " +
                        "<a " +
                        "class=\"gathererTooltip\" " +
                        "data-image=\"https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card\" " +
                        "data-width=\"223px\"" +
                        "data-height=\"311px\"" +
                        "href=\"https://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=" + card.multiverseId + "\"" +
                        ">" + card.name + "</a><br/>";
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
