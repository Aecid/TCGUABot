using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot.Types;

namespace TCGUABot.Controllers
{
    //[Route("api/[controller]")]
    //[ApiController]
    public class UpdateController : ControllerBase
    {
        public string HealthCheck()
        {
            return "ALIVE!";
        }

        public async Task<OkResult> Webhook([FromBody]Update update)
        {
            Console.WriteLine("UPDATE CAME!");
            var commands = Bot.Commands;
            var handlers = Bot.Handlers;
            var client = await Bot.Get();

            if (update.CallbackQuery != null)
            {
                if (update.CallbackQuery.Data != null)
                {
                    foreach (var handler in handlers)
                    {
                        if (handler.Is(update.CallbackQuery.Data))
                        {
                            Console.WriteLine("Result: {0}, {1}", update.CallbackQuery.Data, update.CallbackQuery.From.Id);
                            handler.Execute(update.CallbackQuery, client);
                        }
                    }
                }
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
                                command.Execute(update.Message, client);
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