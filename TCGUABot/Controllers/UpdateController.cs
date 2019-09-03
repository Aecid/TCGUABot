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
    public class UpdateController : ControllerBase
    {
        public ApplicationDbContext context { get; set; }
        public UpdateController(ApplicationDbContext context)
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
            var client = await Bot.Get();
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
                            handler.Execute(update.CallbackQuery, client, context);
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
                    await client.ForwardMessageAsync("-1001202180806", update.Message.Chat.Id, update.Message.MessageId, true);
                    if (update.Message.Text != null)
                    {
                        foreach (var command in commands)
                        {
                            if (command.Contains(update.Message.Text))
                            {
                                Console.WriteLine("Incoming message from:" + update.Message.From.FirstName + " " + update.Message.From.LastName + " @" + update.Message.From.Username + "("+update.Message.From.Id+"), in chat: " + update.Message.Chat.Title + "("+update.Message.Chat.Id+")\r\n"+ update.Message.Text);
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