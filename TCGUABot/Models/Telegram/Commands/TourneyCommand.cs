using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    public class TourneyCommand : Command
    {
        public override string Name => "/tourney";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var TList = context.Tournaments.Where(t => t.IsClosed == false).ToList();
            var chatId = message.Chat.Id;

            if (TList.Count > 0)
            {
                var msg = string.Empty;
                var keyboardList = new List<List<InlineKeyboardButton>>();

                foreach (var tourney in TList)
                {
                    var buttonList = new List<InlineKeyboardButton>();
                    msg += "<b>" + tourney.PlannedDate;
                    msg += " - ";
                    msg += tourney.Name + "</b>";
                    var tourneyPlayers = context.TournamentUserPairs.Where(p => p.TournamentId == tourney.Id).ToList();

                    if (tourneyPlayers != null && tourneyPlayers.Count > 0)
                    {
                        foreach (var player in tourney.TournamentUserPairs)
                        {
                            msg += "\r\n🧙‍♂️ " + "<a href=\"tg://user?id=" + player.PlayerTelegramId + "\">" + context.TelegramUsers.FirstOrDefault(u => u.Id == player.PlayerTelegramId).Name + "</a>";
                        }
                    }
                    if (TList.Count > 1) msg += "\r\n";

                    buttonList.Add(InlineKeyboardButton.WithUrl(tourney.Name, "https://ace.od.ua/Tournaments/Details?id=" + tourney.Id));
                    buttonList.Add(InlineKeyboardButton.WithCallbackData("✅", "t" + "|" + "1" + "|" + tourney.Id + "|" + message.MessageId));
                    buttonList.Add(InlineKeyboardButton.WithCallbackData("❌", "t" + "|" + "0" + "|" + tourney.Id + "|" + message.MessageId));

                    keyboardList.Add(buttonList);
                }

                var keyboard = new InlineKeyboardMarkup(keyboardList);

                await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard);
            }
        }
    }
}