using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TCGUABot.Data;
using TCGUABot.Data.Models;

namespace TCGUABot.Pages.Tournaments
{
    [Authorize(Roles = "Admin, Store Owner, Judge, Event Organizer")]
    public class DeleteModel : PageModel
    {
        private readonly TCGUABot.Data.ApplicationDbContext _context;

        public DeleteModel(TCGUABot.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Tournament Tournament { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tournament = await _context.Tournaments.FirstOrDefaultAsync(m => m.Id == id);

            if (Tournament == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Tournament = await _context.Tournaments.FindAsync(id);

            if (Tournament != null)
            {
                var entities = _context.TournamentUserPairs.Where(tu => tu.TournamentId == Tournament.Id);
                _context.TournamentUserPairs.RemoveRange(entities);
                _context.Tournaments.Remove(Tournament);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
