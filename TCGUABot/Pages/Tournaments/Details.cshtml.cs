using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace TCGUABot.Pages.Tournaments
{
    public class DetailsModel : PageModel
    {
        private readonly TCGUABot.Data.ApplicationDbContext _context;
        private readonly UserManager<Data.Models.ApplicationUser> _userManager;

        public DetailsModel(TCGUABot.Data.ApplicationDbContext context, UserManager<Data.Models.ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public Tournament Tournament { get; set; }
        public List<Deck> PlayerDecks { get; set; }
        public ApplicationUser CurrentUser { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tournament = await _context.Tournaments.FirstOrDefaultAsync(m => m.Id == id);

            if (User.Identity.IsAuthenticated)
            {
                CurrentUser = await _userManager.GetUserAsync(User);
                CurrentUser.TelegramId = _context.UserLogins.FirstOrDefault(l => l.UserId == CurrentUser.Id)?.ProviderKey;
                PlayerDecks = _context.Decks.Where(o => o.UserId == CurrentUser.Id).OrderByDescending(z => z.CreationDate).ToList();
            }


            if (Tournament == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
