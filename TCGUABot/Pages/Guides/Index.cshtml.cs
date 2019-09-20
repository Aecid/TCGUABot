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
    public class IndexModel : PageModel
    {
        private readonly TCGUABot.Data.ApplicationDbContext _context;

        public IndexModel(TCGUABot.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<DeckGuide> DeckGuide { get;set; }

        public async Task OnGetAsync()
        {
            DeckGuide = await _context.DeckGuides.ToListAsync();
        }
    }
}
