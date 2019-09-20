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

namespace TCGUABot.Pages.Guides
{
    [Authorize(Roles = "Admin, Black Sea Team Member")]
    public class EditModel : PageModel
    {
        private readonly TCGUABot.Data.ApplicationDbContext _context;

        public EditModel(TCGUABot.Data.ApplicationDbContext context)
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

            DeckGuide.KeywordsSeparated = string.Join(",", DeckGuide.Keywords);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            DeckGuide.LastUpdated = TimeService.GetLocalTime();
            var kwValues = Request.Form["DeckGuide.KeywordsSeparated"].ToString();
            var kwArray = kwValues.Split(",");
            List<string> keyWords = new List<string>();
            foreach (var value in kwArray)
            {
                keyWords.Add(value.Trim());
            }

            DeckGuide.Keywords = keyWords.ToArray();
            _context.Attach(DeckGuide).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeckGuideExists(DeckGuide.Id))
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

        private bool DeckGuideExists(int id)
        {
            return _context.DeckGuides.Any(e => e.Id == id);
        }
    }
}
