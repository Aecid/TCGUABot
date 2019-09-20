using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class SettingsCommand : Command
    {
        public override string Name => "/settings";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var isPrivate = message.Chat.Id == message.From.Id;
            var msg = string.Empty;

            ChatMember[] admins = new ChatMember[0];
            if (!isPrivate)
            {
                admins = await client.GetChatAdministratorsAsync(message.Chat.Id);
            }
            if (admins.Any(a => a.User.Id == message.From.Id) || message.From.Id == 186070199 || isPrivate)
            {
                try
                {
                    string[] langs = { "ru", "en", "ua" };
                    string[] bools = { "true", "false" };

                    if (message.Text.Contains(" set "))
                    {
                        if (message.Text.Contains(" lang "))
                        {
                            var text = message.Text.Replace("/settings set lang ", "").Trim();
                            if (langs.Contains(text))
                            {
                                msg += "<b>lang</b> was set to <b>" + text + "</b>\r\n";
                                context.TelegramChats.FirstOrDefault(tc => tc.Id == message.Chat.Id).Language = text;
                            }
                            else
                            {
                                var errormsg = "Wrong 'lang' value :(";
                                await client.SendTextMessageAsync(message.Chat.Id, errormsg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
                            }
                        }

                        if (message.Text.Contains(" sendSpoilers "))
                        {
                            var text = false;

                            if (bool.TryParse(message.Text.Replace("/settings set sendSpoilers ", "").Trim(), out text))
                            {
                                msg += "<b>sendSpoilers</b> was set to <b>" + text.ToString() + "</b>\r\n";
                                context.TelegramChats.FirstOrDefault(tc => tc.Id == message.Chat.Id).SendSpoilers = text;
                            }
                            else
                            {
                                var errormsg = "Wrong 'sendSpoilers' value :(";
                                await client.SendTextMessageAsync(message.Chat.Id, errormsg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
                            }
                        }
                    }
                    else
                    {
                        msg += "Usage: <b>/settings set {setting} {value}</b>\r\nExample: /settings set lang ua\r\n";
                    }


                    var chatData = context.TelegramChats.FirstOrDefault(tc => tc.Id == message.Chat.Id);
                    msg += "<b>Chat settings:</b>\r\n" +
                        "<b>lang</b> (en/ua/ru): <b>" + chatData.Language + "</b>\r\n" +
                        "<b>sendSpoilers</b> (true/false): <b>" + chatData.SendSpoilers + "</b>";

                    context.SaveChanges();
                    await client.SendTextMessageAsync(chatData.Id, msg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
                }
                catch { }
            }
            else
            {
                msg = "You have to be admin in chat \""+ message.Chat.Title +"\" to use this command";
                await client.SendTextMessageAsync(message.Chat.Id, msg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
            }
        }
    }
}
