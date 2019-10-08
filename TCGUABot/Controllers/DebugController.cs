using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCGUABot.Data;
using TCGUABot.Models;
using TCGUABot.Models.Commands;
using TCGUABot.Models.InlineQueryHandler;
using Telegram.Bot.Types;

namespace TCGUABot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public DebugController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [HttpGet("/Debug/Card/{cardName}", Name = "DebugCard")]
        public string CardCommand(string cardName)
        {
            var result = string.Empty;
            var list = new List<Card>();
            var sets = CardData.Instance.Sets.Where(s => s.cards.Any(c => c.name.Equals(cardName, StringComparison.InvariantCultureIgnoreCase)));
            foreach (var set in sets)
            {
                var cards = set.cards.Where(c => c.name.Equals(cardName, StringComparison.InvariantCultureIgnoreCase));
                foreach (var card in cards)
                {
                    list.Add(card);
                }
            }

            foreach (var card in list)
            {
                result += JsonConvert.SerializeObject(card, Formatting.Indented) + "\r\n";
            }

            return result;
        }
    }
}