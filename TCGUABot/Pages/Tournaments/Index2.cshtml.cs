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
    public class Index2Model : PageModel
    {
        private readonly ApplicationDbContext _context;

        public Index2Model(ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Tournament> Tournament { get;set; }
        public IList<Tournament> OutdatedTournament { get; set; }


        public async Task OnGetAsync()
        {
            Tournament = await _context.Tournaments.Where(t => DateTime.Compare(t.PlannedDate.AddHours(10), TimeService.GetLocalTime()) > 0).OrderBy(t => t.PlannedDate).ToListAsync();
            OutdatedTournament = await _context.Tournaments.Where(t => DateTime.Compare(t.PlannedDate.AddHours(10), TimeService.GetLocalTime()) <= 0).OrderByDescending(t => t.PlannedDate).ToListAsync();
        
            foreach (var tourney in Tournament)
            {
                tourney.RegisteredPlayers = new List<TelegramUser>();
                var pairList = _context.TournamentUserPairs.Where(u => u.TournamentId == tourney.Id).ToList();
                foreach (var pair in pairList)
                {
                    tourney.RegisteredPlayers.Add(_context.TelegramUsers.FirstOrDefault(u => u.Id == pair.PlayerTelegramId));
                }

            }

            //foreach (var tourney in OutdatedTournament)
            //{
            //    tourney.RegisteredPlayers = new List<TelegramUser>();
            //    var pairList = _context.TournamentUserPairs.Where(u => u.TournamentId == tourney.Id).ToList();
            //    foreach (var pair in pairList)
            //    {
            //        tourney.RegisteredPlayers.Add(_context.TelegramUsers.FirstOrDefault(u => u.Id == pair.PlayerTelegramId));
            //    }

            //}
        }
    }
}
