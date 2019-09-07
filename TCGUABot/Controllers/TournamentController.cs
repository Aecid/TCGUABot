using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using TCGUABot.Helpers;
using TCGUABot.Models;
using TCGUABot.Models.Commands;
using Telegram.Bot.Types;
using Z.EntityFramework.Plus;

namespace TCGUABot.Controllers
{
    public class TournamentController : Controller
    {
        ApplicationDbContext context { get; set; }
        UserManager<ApplicationUser> userManager { get; set; }
        public TournamentController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.userManager = userManager;
        }

        [HttpGet]
        public JsonResult GetTournamentPlayers([FromQuery] string tourneyId)
        {
            var registeredPlayers = new List<TelegramUser>();
            var pairList = context.TournamentUserPairs.Where(u => u.TournamentId == tourneyId).ToList();
            foreach (var pair in pairList)
            {
                registeredPlayers.Add(context.TelegramUsers.FirstOrDefault(u => u.Id == pair.PlayerTelegramId));
            }

            var result = new List<string>();

            foreach (var player in registeredPlayers)
            {
                result.Add(player.EmojiStatus + "&nbsp;" + player.Name.Replace(" ", "&nbsp;"));
            }

            return Json(result);
        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult> TogglePlayerParticipation([FromQuery] string tourneyId, [FromQuery] string playerId)
        {
            var playerTelegramId = context.UserLogins.FirstOrDefault(u => u.UserId == playerId)?.ProviderKey;
            if (playerTelegramId == null) return Forbid();

            var player = context.TelegramUsers.FirstOrDefault(u => u.Id == long.Parse(playerTelegramId));

            var playerToCheck = context.TournamentUserPairs.FirstOrDefault(p => p.PlayerTelegramId == long.Parse(playerTelegramId) && p.TournamentId == tourneyId);

            if (playerToCheck!=null)
            {
                context.TournamentUserPairs.Remove(playerToCheck);
            }
            else
            {
                var user = await userManager.GetUserAsync(User);
                var pair = new TournamentUserPair()
                {
                    PlayerId = user.Id,
                    PlayerTelegramId = long.Parse(playerTelegramId),
                    TournamentId = tourneyId
                };
                context.TournamentUserPairs.Add(pair);
            }
            context.SaveChanges();
            return Ok();
        }

        [Authorize(Roles = "Admin, Store Owner, Judge, Event Organizer")]
        [HttpGet]
        public async Task<ActionResult> CreateDefaultTournament([FromQuery] string type, string creator)
        {
            DateTime now = TimeService.GetLocalTime();
            var tourneyTime = new DateTime(now.Year, now.Month, now.Day, 11, 00, 00);
            string name = "";

            if (type == "sat")
            {
                tourneyTime = tourneyTime.Next(DayOfWeek.Saturday);
                name = "Modern - Corvin";
            }
            
            if (type == "sun")
            {
                tourneyTime = tourneyTime.Next(DayOfWeek.Sunday);
                name = "Limited - Corvin";
            }

            if (!context.Tournaments.Any(d => d.PlannedDate == tourneyTime))
            {
                var tourney = new Tournament()
                {
                    PlannedDate = tourneyTime,
                    CreationDate = TimeService.GetLocalTime(),
                    CreatorId = creator,
                    Name = name,
                    Description = "Default tournament"
                };

                context.Tournaments.Add(tourney);
                context.SaveChanges();

                return Redirect("/Tournaments");
            }
            else
            {
                ModelState.AddModelError("", "Tournament already exists");
                return BadRequest(ModelState);
            }
        }
    }
}
