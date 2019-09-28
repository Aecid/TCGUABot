using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
            Console.WriteLine(JsonConvert.SerializeObject(update));

            var z = Request.Body;

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
                            await handler.Execute(update.CallbackQuery, client, context);
                        }
                    }
                }
            }

            if (update.InlineQuery != null)
            {
                await inlineQueryHandler.Execute(update.InlineQuery, client);
            }

            if (update != null)
            {
                if (update.Message != null)
                {
                    if (update.Message.Chat != null)
                    {
                        Helpers.TelegramUtil.AddChat(update.Message.Chat.Id, context, update.Message.Chat.Id == update.Message.From.Id);
                    }

                    if (update.Message.Text != null)
                    {
                        foreach (var command in commands)
                        {
                            if (command.Contains(update.Message.Text))
                            {
                                try
                                {
                                    var user = update.Message.From.FirstName + " " + update.Message.From.LastName + " @" + update.Message.From.Username;
                                    var userId = update.Message.From.Id;
                                    var chatId = update.Message.Chat.Id;
                                    var chatName = userId == chatId ? "Private" : update.Message.Chat.Title;
                                    var text = update.Message.Text;
                                    var messageText = String.Format("Bot: @tcgua_bot, incoming from: {0} ({1}), chat {2} ({3}), msg: {4}", user, userId, chatName, chatId, text);
                                    //Logging?:D
                                    await client.SendTextMessageAsync("-1001202180806", messageText, Telegram.Bot.Types.Enums.ParseMode.Html, true, true);
                                }
                                catch { }

                                Console.WriteLine("Incoming message from:" + update.Message.From.FirstName + " " + update.Message.From.LastName + " @" + update.Message.From.Username + "("+update.Message.From.Id+"), in chat: " + update.Message.Chat.Title + "("+update.Message.Chat.Id+")\r\n"+ update.Message.Text);
                                await command.Execute(update.Message, client, context);
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