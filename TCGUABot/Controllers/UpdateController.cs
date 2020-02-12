using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCGUABot.Data;
using TCGUABot.Models.Commands;
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
                            var user = update.CallbackQuery.From.FirstName + " " + update.CallbackQuery.From.LastName + " @" + update.CallbackQuery.From.Username;
                            var userId = update.CallbackQuery.From.Id;
                            var text = update.CallbackQuery.Data;
                            var messageText = String.Format("Bot: @tcgua_bot, incoming from: {0} ({1}), msg: {2}", user, userId, text);
                            //Logging?:D
                            try
                            {
                                await client.SendTextMessageAsync("-1001202180806", messageText, Telegram.Bot.Types.Enums.ParseMode.Html, true, true);
                            }
                            catch { }
                            Console.WriteLine("Result: {0}, {1}", update.CallbackQuery.Data, update.CallbackQuery.From.Id);
                            await handler.Execute(update.CallbackQuery, client, context);
                        }
                    }
                }
            }

            if (update.InlineQuery != null)
            {
                await inlineQueryHandler.Execute(update.InlineQuery, client, context);
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
                            if (command.StartsWith(update.Message.Text))
                            {
                                if (!context.CatifiedUsers.Any(z => z.TelegramId == update.Message.From.Id))
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

                                    Console.WriteLine("Incoming message from:" + update.Message.From.FirstName + " " + update.Message.From.LastName + " @" + update.Message.From.Username + "(" + update.Message.From.Id + "), in chat: " + update.Message.Chat.Title + "(" + update.Message.Chat.Id + ")\r\n" + update.Message.Text);
                                    try
                                    {
                                        await command.Execute(update.Message, client, context);
                                    }
                                    catch
                                    {
                                        var errorMsg = "Error executing command " + command.Name;
                                        await client.SendTextMessageAsync("-1001202180806", errorMsg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true);
                                    }
                                    break;
                                }
                                else
                                {
                                    var catCommand = new TranslateCommand();
                                    await catCommand.Execute(new Message() { Chat = new Chat() { Id = update.Message.Chat.Id }, ReplyToMessage = update.Message }, client, context);
                                    break;
                                }
                            }
                        }

                    }
                }
            }

            return Ok();
        }

        [HttpGet("/Update/UpdateCardsDB", Name = "UpdateCardsDB")]
        public string UpdateCardsDB()
        {
            var z = CardData.TcgGroups;
            var res = string.Empty;

            foreach (var group in z)
            {
                try
                {
                    var cards = CardData.TcgGetGroupContentById(group.groupId);
                    foreach (var card in cards)
                    {
                        var p = new Product()
                        {
                            ProductId = card.productId,
                            Name = card.name,
                            CleanName = card.cleanName,
                            GroupId = card.groupId,
                            Url = card.url,
                            ImageUrl = card.imageUrl,
                            ExtendedData = JsonConvert.SerializeObject(card.extendedData)
                        };

                        try
                        {
                            context.Cards.Add(p);
                        }
                        catch
                        {

                        }
                    }

                    context.SaveChanges();
                }
                catch { }
            }


            return res;
        }
    }
}