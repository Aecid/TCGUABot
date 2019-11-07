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
                try
                {
                    admins = await client.GetChatAdministratorsAsync(message.Chat.Id);
                }
                catch { }
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
                                try
                                {
                                    await client.SendTextMessageAsync(message.Chat.Id, errormsg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
                                }
                                catch { }
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
                                try
                                {
                                    await client.SendTextMessageAsync(message.Chat.Id, errormsg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
                                }
                                catch { }
                            }
                        }

                        if (message.Text.Contains(" broadcast "))
                        {
                            var text = false;
                            if (message.Chat.Id == message.From.Id)
                            {

                                if (bool.TryParse(message.Text.Replace("/settings set broadcast ", "").Trim(), out text))
                                {
                                    msg += "<b>sendSpoilers</b> was set to <b>" + text.ToString() + "</b>\r\n";
                                    context.TelegramUsers.FirstOrDefault(tc => tc.Id == message.Chat.Id).AcceptBroadcast = text;
                                }
                                else
                                {
                                    var errormsg = "Wrong 'broadcast' value :(";

                                    try 
                                    {
                                        await client.SendTextMessageAsync(message.Chat.Id, errormsg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
                                    }
                                    catch { }
                                }
                            }
                            else
                            {
                                try
                                {
                                    await client.SendTextMessageAsync(message.Chat.Id, "This setting works only for private chats");
                                }
                                catch { }
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

                    if (message.Chat.Id == message.From.Id)
                    {
                        var userData = context.TelegramUsers.FirstOrDefault(tc => tc.Id == message.Chat.Id);
                        msg += "\r\n\r\n<b>Private settings:</b>\r\n<b>broadcast</b> (true/false): <b>" + userData.AcceptBroadcast + "</b>";
                    }

                    context.SaveChanges();
                    try
                    {
                        await client.SendTextMessageAsync(chatData.Id, msg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
                    }
                    catch { }
                }
                catch { }
            }
            else
            {
                msg = "You have to be admin in chat \""+ message.Chat.Title +"\" to use this command";
                try
                {
                    await client.SendTextMessageAsync(message.Chat.Id, msg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
                }
                catch { }
            }
        }
    }
}
