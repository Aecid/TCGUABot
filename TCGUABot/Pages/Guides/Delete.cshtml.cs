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

namespace TCGUABot.Pages.Guides
{
    [Authorize(Roles = "Admin, Black Sea Team Member")]
    public class DeleteModel : PageModel
    {
        private readonly TCGUABot.Data.ApplicationDbContext _context;

        public DeleteModel(TCGUABot.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DeckGuide DeckGuide { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DeckGuide = await _context.DeckGuides.FirstOrDefaultAsync(m => m.Id == id);

            if (DeckGuide == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DeckGuide = await _context.DeckGuides.FindAsync(id);

            if (DeckGuide != null)
            {
                _context.DeckGuides.Remove(DeckGuide);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
