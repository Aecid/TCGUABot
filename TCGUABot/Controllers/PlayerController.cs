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
using TCGUABot.Models;
using TCGUABot.Models.Commands;
using Telegram.Bot.Types;
using Z.EntityFramework.Plus;

namespace TCGUABot.Controllers
{
    public class PlayerController : Controller
    {
        ApplicationDbContext context { get; set; }
        UserManager<ApplicationUser> userManager { get; set; }
        public PlayerController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
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
    }
}
