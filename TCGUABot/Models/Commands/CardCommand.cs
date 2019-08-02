using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class CardCommand : Command
    {
        public override string Name => "/c";

        public override async void Execute(Message message, TelegramBotClient client)
        {
            var text = message.Text.Replace("/c ", "");
            var msg = string.Empty;
            Card card = new Card();
            var chatId = message.Chat.Id;
            var set = CardData.Instance.Sets.FirstOrDefault(s => s.Value.cards.Any(c => c.name.StartsWith(text))).Value;
            if (set == null)
            {
                set = CardData.Instance.Sets.FirstOrDefault(s => s.Value.cards.Any(c => c.foreignData.Any(f => f.name.StartsWith(text)))).Value;
            }
            if (set != null)
            {
                card = set.cards.FirstOrDefault(c => c.name.StartsWith(text));
                msg += "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card";
            }
            else
            {
                msg = "Карта не найдена.";
            }

            await client.SendTextMessageAsync(chatId, msg);
        }
    }
}