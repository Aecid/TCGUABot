using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
            bool side = false;
            foreach (var myString in text.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                ArenaCard card = new ArenaCard();

                if (myString.Length > 0)
                {
                    var cardData = myString.Split(" ");
                    int.TryParse(cardData[0], out card.count);
                    card.name = cardData[1].ToString();
                    card.set = cardData[2].ToString();
                    int.TryParse(cardData[3], out card.collectorNumber);

                    if (side) deck.SideBoard.Add(card);
                    else deck.MainDeck.Add(card);
                }

                else
                {
                    side = true;
                }
            }

            var controller = new TCGUABot.Controllers.DecklistController();
            var id = controller.Post(deck);

            var chatId = message.Chat.Id;
            await client.SendTextMessageAsync(chatId, "...here goes nothing: https://ace.od.ua:8443/decks/"+id);
        }
    }
}