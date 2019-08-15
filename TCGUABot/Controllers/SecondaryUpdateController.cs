using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCGUABot.Data;
using TCGUABot.Models.InlineQueryHandler;
using Telegram.Bot.Types;

namespace TCGUABot.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class SecondaryUpdateController : ControllerBase
    {
        public ApplicationDbContext context { get; set; }
        public SecondaryUpdateController(ApplicationDbContext context)
        {
            this.context = context;
        }

        public string HealthCheck()
        {
            return "ALIVE!";
        }

        public async Task<OkResult> Webhook([FromBody]Update update)
        {
            var commands = Bot.Commands;
            var callbackHandlers = Bot.CallbackHandlers;
            var client = await SecondaryBot.Get();
            var inlineQueryHandler = new InlineQueryHandler();
            if (update.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data != null)
                {
                    foreach (var handler in callbackHandlers)
                    {
                        if (handler.Is(update.CallbackQuery.Data))
                        {
                            Console.WriteLine("Result: {0}, {1}", update.CallbackQuery.Data, update.CallbackQuery.From.Id);
                            handler.Execute(update.CallbackQuery, client);
                        }
                    }
                }
            }

            if (update.InlineQuery != null)
            {
                inlineQueryHandler.Execute(update.InlineQuery, client);
            }

            if (update != null)
            {
                if (update.Message != null)
                {
                    if (update.Message.Text != null)
                    {
                        foreach (var command in commands)
                        {
                            if (command.Contains(update.Message.Text))
                            {
                                Console.WriteLine(update.Message.From.FirstName + " " + update.Message.From.LastName + " @" + update.Message.From.Username + ": " + update.Message.Text);
                                command.Execute(update.Message, client, context);
                                break;
                            }
                        }
                    }
                }
            }

            return Ok();
        }
    }
}