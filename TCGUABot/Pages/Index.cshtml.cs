using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TCGUABot.Models;

namespace TCGUABot.Pages
{
    public class IndexModel : PageModel
    {
        public void OnGet()
        {

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
