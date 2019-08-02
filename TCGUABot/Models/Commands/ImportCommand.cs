using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class ImportCommand : Command
    {
        public override string Name => "/import";

        public override async void Execute(Message message, TelegramBotClient client)
        {
            var text = message.Text.Replace("/import ", "");
            var deck = new DeckArenaImport();
            deck.MainDeck = new List<ArenaCard>();
            deck.SideBoard = new List<ArenaCard>();
            bool side = false;
            foreach (var myString in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                ArenaCard card = new ArenaCard();

                if (myString.Trim().Length > 1)
                {
                    var cardData = myString.Split(" ");
                    Regex exp = new Regex(@"(\d+)\s+(.*)\s+(\(.+\))\s+(\d+)");
                    var matches = exp.Matches(myString);
                
                    int.TryParse(matches[0].Groups[1].Value, out card.count);
                    card.name = matches[0].Groups[2].Value;
                    card.set = matches[0].Groups[3].Value;
                    int.TryParse(matches[0].Groups[4].Value, out card.collectorNumber);

                    if (side) deck.SideBoard.Add(card);
                    else deck.MainDeck.Add(card);
                }

                else
                {
                    side = true;
                }
            }

            var controller = new TCGUABot.Controllers.DecklistController();
            var id = controller.Import(deck);

            var chatId = message.Chat.Id;
            await client.SendTextMessageAsync(chatId, "...here goes nothing: https://ace.od.ua:8443/decks/"+id);
        }
    }
}