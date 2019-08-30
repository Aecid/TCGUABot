using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using TCGUABot.Models.CallbackHandlers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.CallbackHandlers
{
    public class TourneyCallbackHandler : CallbackHandler
    {
        public override string Name => "t";

        public override async void Execute(CallbackQuery query, TelegramBotClient client, ApplicationDbContext context)
        {
            //                    buttonList.Add(InlineKeyboardButton.WithCallbackData("✅", "t" + "|" + "1" + "|" + tourney.Id + "|" + message.MessageId));
            var dataArray = query.Data.Split("|");
            var name = dataArray[0];
            var value = dataArray[1];
            var tourneyId = dataArray[2];
            var messageId = dataArray[3];

            var isExistingUser = context.TelegramUsers.Any(u => u.Id == query.From.Id);
            if (!isExistingUser)
            {
                var user = new TelegramUser()
                {
                    Id = query.From.Id,
                    FirstName = query.From.FirstName,
                    LastName = query.From.LastName,
                    Username = query.From.Username,
                    EmojiStatus = "🧙‍♂️"
                };

                context.TelegramUsers.Add(user);
                context.SaveChanges();
            }
            else
            {
                var existingUser = context.TelegramUsers.FirstOrDefault(u => u.Id == query.From.Id);
                var areChanges = false;
                if (existingUser.FirstName != query.From.FirstName)
                {
                    areChanges = true;
                    existingUser.FirstName = query.From.FirstName;
                }
                if (existingUser.LastName != query.From.LastName)
                {
                    areChanges = true;
                    existingUser.LastName = query.From.LastName;
                }
                if (existingUser.Username != query.From.Username)
                {
                    areChanges = true;
                    existingUser.Username = query.From.Username;
                }

                if (areChanges)
                {
                    context.SaveChanges();
                }
            }

            if (value == "1") //register for tourney
            {

                var tourney = context.Tournaments.FirstOrDefault(t => t.Id == tourneyId);

                if (context.TournamentUserPairs.Any(p => p.TournamentId == tourneyId))
                {
                    if (!context.TournamentUserPairs.Any(p => p.PlayerTelegramId == query.From.Id && p.TournamentId == tourneyId))
                    {
                        var firstPId = context.UserLogins.FirstOrDefault(l => l.ProviderKey == query.From.Id.ToString());
                        var playerId = firstPId == null ? "0" : firstPId.UserId;
                        context.TournamentUserPairs.Add(new TournamentUserPair() { PlayerTelegramId = query.From.Id, DeckId="", PlayerId = playerId, TournamentId = tourneyId });
                        context.SaveChanges();
                    }
                }
                else
                {
                    var firstPId = context.UserLogins.FirstOrDefault(l => l.ProviderKey == query.From.Id.ToString());
                    var playerId = firstPId == null ? "0" : firstPId.UserId;
                    context.TournamentUserPairs.Add(new TournamentUserPair() { PlayerTelegramId = query.From.Id, DeckId = "", PlayerId = playerId, TournamentId = tourneyId });
                    context.SaveChanges();
                }
            }

            if (value == "0")
            {
                var tourney = context.Tournaments.FirstOrDefault(t => t.Id == tourneyId);

                    if (context.TournamentUserPairs.Any(p => p.PlayerTelegramId == query.From.Id && p.TournamentId == tourneyId))
                    {
                        var userPair = context.TournamentUserPairs.FirstOrDefault(u => u.PlayerTelegramId == query.From.Id && u.TournamentId == tourneyId);
                        context.TournamentUserPairs.Remove(userPair);
                        context.SaveChanges();
                    }
                
            }

            var TList = context.Tournaments.Where(t => t.IsClosed == false).ToList();
            var chatId = query.Message.Chat.Id;

            var msg = string.Empty;

            if (TList.Count > 0)
            {
                var keyboardList = new List<List<InlineKeyboardButton>>();

                foreach (var tourney in TList)
                {
                    var buttonList = new List<InlineKeyboardButton>();

                    msg += "<b>" + tourney.PlannedDate;
                    msg += " - ";
                    msg += tourney.Name + "</b>";
                    var tourneyPlayers = context.TournamentUserPairs.Where(p => p.TournamentId == tourney.Id).ToList();
                    if (tourneyPlayers.Count > 0)
                    {
                        foreach (var player in tourneyPlayers)
                        {

                            var tplayer = context.TelegramUsers.FirstOrDefault(u => u.Id == player.PlayerTelegramId);
                            var status = string.IsNullOrEmpty(tplayer.EmojiStatus) ? "🧙‍♂️" : tplayer.EmojiStatus;
                            msg += "\r\n" + status + tplayer.Name;
                            //msg += "\r\n🧙‍♂️ " + "<a href=\"tg://user?id="+ player.PlayerTelegramId + "\">"+ context.TelegramUsers.FirstOrDefault(u => u.Id == player.PlayerTelegramId).Name +"</a>";
                        }
                    }
                    //msg += "\r\n<i>" + tourney.Description + "</i>";
                    if (TList.Count > 1) msg += "\r\n";

                    buttonList.Add(InlineKeyboardButton.WithUrl(tourney.Name, "https://ace.od.ua/Tournaments/Details?id=" + tourney.Id));
                    buttonList.Add(InlineKeyboardButton.WithCallbackData("✅", "t" + "|" + "1" + "|" + tourney.Id + "|" + query.Message.MessageId));
                    buttonList.Add(InlineKeyboardButton.WithCallbackData("❌", "t" + "|" + "0" + "|" + tourney.Id + "|" + query.Message.MessageId));

                    keyboardList.Add(buttonList);
                }

                var keyboard = new InlineKeyboardMarkup(keyboardList);

                try
                {
                    await client.EditMessageTextAsync(query.Message.Chat.Id, query.Message.MessageId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard);
                }
                catch { }
            }
        }
    }
}
