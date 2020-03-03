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
            var result = new List<string>();

            var registeredPlayers = GetTournamentPlayersAsList(tourneyId);

            foreach (var player in registeredPlayers)
            {
                result.Add(player.EmojiStatus + "&nbsp;" + player.Name.Replace(" ", "&nbsp;"));
            }

            return Json(result);
        }

        public List<TelegramUser> GetTournamentPlayersAsList(string tourneyId)
        {
            var registeredPlayers = new List<TelegramUser>();
            var pairList = context.TournamentUserPairs.Where(u => u.TournamentId == tourneyId).ToList();
            foreach (var pair in pairList)
            {
                registeredPlayers.Add(context.TelegramUsers.FirstOrDefault(u => u.Id == pair.PlayerTelegramId));
            }

            return registeredPlayers;
        }

        [HttpGet]
        public ActionResult GetTournamentPlayersWithDetails([FromQuery] string tourneyId, [FromQuery] string playerId)
        {
            try
            {
                var isJudge = User.Identity.IsAuthenticated && User.IsInRole("Judge");
                var players = GetTournamentPlayersAsList(tourneyId);
                var playerDeckPairs = context.TournamentUserPairs.Where(p => p.TournamentId == tourneyId);
                string telegramId = "";
                try
                {
                    telegramId = context.UserLogins.FirstOrDefault(l => l.UserId == playerId)?.ProviderKey;
                }
                catch { }
                var result = new List<ExpandoObject>();
                foreach (var player in players)
                {
                    dynamic pl = new ExpandoObject();
                    pl.player = player;
                    var isCurrent = player.Id.ToString() == telegramId;
                    pl.current = isCurrent;
                    var playerDeck = playerDeckPairs.FirstOrDefault(p => p.PlayerTelegramId == player.Id);

                    if (playerDeck != null && !string.IsNullOrEmpty(playerDeck.DeckId))
                    {
                        if (isJudge || isCurrent)
                        {
                            pl.deckId = playerDeck.DeckId;
                            pl.deckName = context.Decks.FirstOrDefault(d => d.Id == playerDeck.DeckId).Name;
                            pl.hasDeck = true;
                        }
                    }
                    else pl.hasDeck = false;

                    result.Add(pl);
                }

                return Ok(result);
            }
            catch
            {
                return BadRequest();
            }

            //foreach (var player in players)
            //{
            //    html += "<div class=\"d-sm-table-row\">";
            //    html += "<div class=\"d-sm-table-cell\">";
            //    html += player;
            //    html += "</div>";
            //    html += "<div class=\"d-sm-table-cell\">";
            //    if (telegramId == null) html += "Not registered";
            //    if (player.Id.ToString() == telegramId)
            //    {
            //        html += "<button/>";
            //    }
            //    html += "</div>";
            //    html += "<div class=\"d-sm-table-cell\">";
            //    if (telegramId!=null)
            //    {
            //        if (context.TournamentUserPairs.FirstOrDefault(t => t.Id.ToString() == tourneyId && t.PlayerTelegramId.ToString() == telegramId).DeckId!=null)
            //        {
            //            html += "Here comes add deck button";
            //        }
            //        else
            //        {
            //            html += "<a href=\"/Decks/Details?id=abdd4a8f-7ce6-4d09-8695-457d4aa6ed23\">Deck</a>";
            //        }
            //    }
            //    html += "</div>";
            //    html += "</div>";
            //}

            //return html;
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

        [Authorize]
        [HttpGet]
        public ActionResult SetDeck([FromQuery] string tourneyId, [FromQuery] string playerId, [FromQuery] string deckId)
        {
            var playerTelegramId = context.UserLogins.FirstOrDefault(u => u.UserId == playerId)?.ProviderKey;
            if (playerTelegramId == null) return Forbid();

            var player = context.TelegramUsers.FirstOrDefault(u => u.Id == long.Parse(playerTelegramId));

            try
            {
                context.TournamentUserPairs.FirstOrDefault(p => p.PlayerTelegramId == long.Parse(playerTelegramId) && p.TournamentId == tourneyId).DeckId = deckId;
                context.SaveChanges();
            }
            catch
            {
                return BadRequest();
            }

            return Ok();
        }

        [Authorize(Roles = "Admin, Store Owner, Judge, Event Organizer")]
        [HttpGet]
        public ActionResult CreateDefaultTournament([FromQuery] string type, string creator)
        {
            DateTime now = TimeService.GetLocalTime();
            var tourneyTime = new DateTime(now.Year, now.Month, now.Day, 12, 00, 00);
            string name = "";

            if (type == "sat")
            {
                tourneyTime = tourneyTime.Next(DayOfWeek.Saturday);
                name = "Modern";
            }
            
            if (type == "sun")
            {
                tourneyTime = tourneyTime.Next(DayOfWeek.Sunday);
                name = "Limited";
            }

            if (!context.Tournaments.Any(d => d.PlannedDate == tourneyTime))
            {
                var tourney = new Tournament()
                {
                    PlannedDate = tourneyTime,
                    CreationDate = TimeService.GetLocalTime(),
                    CreatorId = creator,
                    Name = name,
                    LocationId = 1,
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
