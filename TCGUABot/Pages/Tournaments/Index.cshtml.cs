using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TCGUABot.Data;
using TCGUABot.Data.Models;

namespace TCGUABot.Pages.Tournaments
{
    public class IndexModel : PageModel
    {
        private readonly TCGUABot.Data.ApplicationDbContext _context;

        public IndexModel(TCGUABot.Data.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Tournament> Tournament { get;set; }
        public IList<Tournament> OutdatedTournament { get; set; }


        public async Task OnGetAsync()
        {
            Tournament = await _context.Tournaments.Where(t => DateTime.Compare(t.PlannedDate.AddHours(10), TimeService.GetLocalTime()) > 0).OrderBy(t => t.PlannedDate).ToListAsync();
            OutdatedTournament = await _context.Tournaments.Where(t => DateTime.Compare(t.PlannedDate.AddHours(10), TimeService.GetLocalTime()) <= 0).OrderByDescending(t => t.PlannedDate).ToListAsync();
            var z = TimeService.GetLocalTime();
            var z2 = z.AddHours(10);
            var z1 = OutdatedTournament[0].PlannedDate;
            var z3 = DateTime.Compare(z1, z2);
            var z4 = 0;
        }
    }
}
