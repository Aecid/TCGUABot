using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCGUABot.Data;
using TCGUABot.Models.Commands;
using Telegram.Bot.Types;

namespace TCGUABot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TestController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [HttpGet("/Test/CardCommand/{cardName}", Name = "CardCommandCall")]
        public async Task<string> CardCommand(string cardName)
        {

            var msg = string.Empty;

            var command = new CardCommand();
            command.Execute(new Message() { Text = "/c " + cardName, Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }

        [HttpGet("/Test/TourneyCommand", Name = "TourneyCommandCall")]
        public async Task<string> TourneyCommand()
        {

            var msg = string.Empty;

            var command = new TourneyCommand();
            command.Execute(new Message() { Text = "/tourney", Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }
    }
}