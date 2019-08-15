using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class OldImportCommand : Command
    {
        public override string Name => "/timport";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var text = message.Text.Replace("/timport ", "");
            var deck = new ImportDeck();
            deck.MainDeck = new List<ImportCard>();
            deck.SideBoard = new List<ImportCard>();
            bool side = false;
            foreach (var myString in text.Split(new string[] { "\r\n" }, StringSplitOptions.None))
            {
                ImportCard card = new ImportCard();

                if (myString.Trim().Length > 1)
                {
                    var cardData = myString.Split(" ");
                    Regex exp = new Regex(@"(\d+)x?\s+(.*)\s*(\(.+\))?\s*(\d+)?");
                    var matches = exp.Matches(myString);
                
                    int.TryParse(matches[0].Groups[1].Value, out card.count);
                    card.name = matches[0].Groups[2].Value;
                    card.set = matches[0].Groups[3].Value.Replace("DAR", "DOM");
                    int.TryParse(matches[0].Groups[4].Value, out card.collectorNumber);

                    if (side) deck.SideBoard.Add(card);
                    else deck.MainDeck.Add(card);
                }

                else
                {
                    side = true;
                }
            }

            var chatId = message.Chat.Id;

            string msg = string.Empty;
            msg += deck.MainDeck.Count + ":" + deck.SideBoard.Count;
            await client.SendTextMessageAsync(chatId, msg);
        }
    }
}