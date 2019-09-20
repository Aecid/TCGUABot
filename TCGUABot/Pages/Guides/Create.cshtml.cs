using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TCGUABot.Data;
using TCGUABot.Data.Models;

namespace TCGUABot.Pages.Guides
{
    [Authorize(Roles = "Admin, Black Sea Team Member")]
    public class CreateModel : PageModel
    {
        private readonly TCGUABot.Data.ApplicationDbContext _context;

        public CreateModel(TCGUABot.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public DeckGuide DeckGuide { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var kwValues = Request.Form["DeckGuide.Keywords"].ToString();
            var kwArray = kwValues.Split(",");
            List<string> keyWords = new List<string>();
            foreach (var value in kwArray)
            {
                keyWords.Add(value.Trim());
            }

            DeckGuide.Keywords = keyWords.ToArray();

            DeckGuide.LastUpdated = TimeService.GetLocalTime();
            _context.DeckGuides.Add(DeckGuide);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}