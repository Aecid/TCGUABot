using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TCGUABot.Data;
using TCGUABot.Data.Models;

namespace TCGUABot.Pages.Tournaments
{
    [Authorize(Roles = "Admin, Store Owner, Judge, Event Organizer")]
    public class EditModel : PageModel
    {
        private readonly TCGUABot.Data.ApplicationDbContext _context;

        public EditModel(TCGUABot.Data.ApplicationDbContext context)
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Tournament).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TournamentExists(Tournament.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool TournamentExists(string id)
        {
            return _context.Tournaments.Any(e => e.Id == id);
        }
    }
}
