using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    public class TourneyCommand : Command
    {
        public override string Name => "/tourney";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var chatId = message.Chat.Id;
            var generatedMessage = GenerateTourneyList(message, context);
            var msg = generatedMessage.Item1;
            var keyboard = generatedMessage.Item2;

            if (!string.IsNullOrEmpty(msg) && keyboard != null)
            {
                var tourneyMessage = await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard, disableNotification: true);
                try //in case of not being an admin in chat
                {
                    await client.PinChatMessageAsync(chatId, tourneyMessage.MessageId, true);
                }
                catch
                {

                }
            }
            else
            {
                var text = "❌<b>Нет анонсированных турниров.</b>";
                try
                {
                    await client.SendTextMessageAsync(chatId, text, disableNotification: true, parseMode: Telegram.Bot.Types.Enums.ParseMode.Html);
                }
                catch { }
            }
        }

        public static Tuple<string, InlineKeyboardMarkup> GenerateTourneyList(Message message, ApplicationDbContext context)
        {
            var TList = context.Tournaments.Where(t => t.IsClosed == false && DateTime.Compare(t.PlannedDate.AddHours(4), TimeService.GetLocalTime()) > 0).OrderBy(t => t.PlannedDate).ToList();
            var chatId = message.Chat.Id;
            var msg = string.Empty;
            var keyboardList = new List<List<InlineKeyboardButton>>();

            var lang = context.TelegramChats.FirstOrDefault(f => f.Id == chatId)?.Language;
            lang = lang == null ? "ru" : lang;

            if (TList.Count > 0)
            {
                foreach (var tourney in TList)
                {
                    var buttonList = new List<InlineKeyboardButton>();
                    msg += "<a href=\"https://ace.od.ua/Tournaments/Details?id=" + tourney.Id + "\">" + string.Format("{0:ddd, dd'/'MM'/'yy HH:mm}", tourney.PlannedDate);
                    msg += " - ";
                    var tname = string.IsNullOrEmpty(tourney.LocationName) ? tourney.Name : tourney.Name + "-" + tourney.LocationName;
                    msg += tname + "</a>";
                    if (!string.IsNullOrEmpty(tourney.EntryFee))
                    {
                        var entryFee = tourney.EntryFee.Contains("бесплатно", StringComparison.InvariantCultureIgnoreCase) ? "🔥<i>бесплатно!</i>🔥" : tourney.EntryFee;
                        entryFee = tourney.EntryFee.Equals("0") ? "🔥<i>" + Lang.Res(lang).free + "</i>🔥" : tourney.EntryFee;
                        msg += "\r\n<b>"+ Lang.Res(lang).entryFee + ": </b>" + entryFee;
                    }
                    if (!string.IsNullOrEmpty(tourney.Rewards)) msg += "\r\n<b>"+ Lang.Res(lang).rewards + ": </b>" + tourney.Rewards;
                    var tourneyPlayers = context.TournamentUserPairs.Where(p => p.TournamentId == tourney.Id).ToList();
                    if (tourneyPlayers != null && tourneyPlayers.Count > 0)
                    {
                        int count = 0;
                        foreach (var player in tourney.TournamentUserPairs)
                        {
                            //msg += "\r\n🧙‍♂️ " + "<a href=\"tg://user?id=" + player.PlayerTelegramId + "\">" + context.TelegramUsers.FirstOrDefault(u => u.Id == player.PlayerTelegramId).Name + "</a>";
                            var tplayer = context.TelegramUsers.FirstOrDefault(u => u.Id == player.PlayerTelegramId);
                            var status = string.IsNullOrEmpty(tplayer.EmojiStatus) ? "🧙‍♂️" : tplayer.EmojiStatus;
                            msg += "\r\n" + (++count) + ". " + status + tplayer.Name;
                        }
                    }
                    if (TList.Count > 1) msg += "\r\n\r\n";

                    //buttonList.Add(InlineKeyboardButton.WithUrl(tourney.Name, "https://ace.od.ua/Tournaments/Details?id=" + tourney.Id));
                    //buttonList.Add(InlineKeyboardButton.WithCallbackData("✅", "t" + "|" + "1" + "|" + tourney.Id + "|" + message.MessageId));
                    //buttonList.Add(InlineKeyboardButton.WithCallbackData("❌", "t" + "|" + "0" + "|" + tourney.Id + "|" + message.MessageId));
                    var formatString = (DateTime.Compare(tourney.PlannedDate, TimeService.GetLocalTime().AddDays(7)) > 0) ? "{0:dd/MM ddd HH:mm}" : "{0:ddd HH:mm}";
                    buttonList.Add(InlineKeyboardButton.WithCallbackData(string.Format(formatString, tourney.PlannedDate) + " " + tourney.Name, "t" + "|" + tourney.Id));

                    keyboardList.Add(buttonList);
                }

            }

            var keyboard = new InlineKeyboardMarkup(keyboardList);

            return new Tuple<string, InlineKeyboardMarkup>(msg, keyboard);
        }
    }
}