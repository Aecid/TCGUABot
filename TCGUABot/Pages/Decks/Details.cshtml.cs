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

        public static string Htmlize(string deck)
        {
            return ImportDeck.HtmlizeString(deck);
        }
    }
}
