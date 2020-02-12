using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Data.Models;
using TCGUABot.Models.CallbackHandlers;
using TCGUABot.Models.Commands;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.CallbackHandlers
{
    public class TourneyCallbackHandler : CallbackHandler
    {
        public override string Name => "t";

        public override async Task Execute(CallbackQuery query, TelegramBotClient client, ApplicationDbContext context)
        {
            //                    buttonList.Add(InlineKeyboardButton.WithCallbackData("✅", "t" + "|" + "1" + "|" + tourney.Id + "|" + message.MessageId));
            var dataArray = query.Data.Split("|");
            var name = dataArray[0];
            var tourneyId = dataArray[1];

            if (tourneyId != "refresh")
            {

                var tUser = query.From;

                Helpers.TelegramUtil.AddUser(tUser, context);

                var tourney = context.Tournaments.FirstOrDefault(t => t.Id == tourneyId);
                if (tourney != null)
                {
                    if (context.TournamentUserPairs.Any(p => p.TournamentId == tourneyId))
                    {
                        if (!context.TournamentUserPairs.Any(p => p.PlayerTelegramId == query.From.Id && p.TournamentId == tourneyId))
                        {
                            var firstPId = context.UserLogins.FirstOrDefault(l => l.ProviderKey == query.From.Id.ToString());
                            var playerId = firstPId == null ? "0" : firstPId.UserId;
                            context.TournamentUserPairs.Add(new TournamentUserPair() { PlayerTelegramId = query.From.Id, DeckId = "", PlayerId = playerId, TournamentId = tourneyId });
                            context.SaveChanges();
                        }
                        else
                        {
                            var userPair = context.TournamentUserPairs.FirstOrDefault(u => u.PlayerTelegramId == query.From.Id && u.TournamentId == tourneyId);
                            context.TournamentUserPairs.Remove(userPair);
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
            }

            var TList = context.Tournaments.Where(t => t.IsClosed == false).ToList();
            var chatId = query.Message.Chat.Id;

            var generatedMesssage = TourneyCommand.GenerateTourneyList(query.Message, context);
            var msg = generatedMesssage.Item1;
            var keyboard = generatedMesssage.Item2;
            
            if (msg.Contains("<i>Last update:</i>"))
                msg = msg.Remove(msg.TrimEnd().LastIndexOf(Environment.NewLine));
            msg += "\r\n<i>Last update:</i> " + "<i>"+TimeService.GetLocalTime().ToString("MM/dd/yyyy HH:mm:ss")+"</i>";

            if (!string.IsNullOrEmpty(msg) && keyboard != null)
            {

                try
                {
                    await client.EditMessageTextAsync(query.Message.Chat.Id, query.Message.MessageId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard);
                }
                catch { }
            }
        }
    }
}
