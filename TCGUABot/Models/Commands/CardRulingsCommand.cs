using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class CardRulingsCommand : Command
    {
        public override string Name => "/cr";

        public override async void Execute(Message message, TelegramBotClient client)
        {
            var text = message.Text.Replace("/cr ", "");
            var msg = string.Empty;
            var chatId = message.Chat.Id;
            var card = Helpers.CardSearch.GetCardByName(text);

            if ( card != null)
            {
                msg += card.name + "\r\n";
                if (card.foreignData.Any(c=>c.language.Equals("Russian"))) msg += card.ruName + "\r\n";
                msg += "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card";

                if (card.rulings.Count>0)
                {
                    msg += "\r\nРулинги: ";
                    foreach (var ruling in card.rulings)
                    {
                        msg += "\r\n"+ruling.date + ": " + ruling.text;
                    }
                }
                else
                {
                    msg += "Рулинги не найдены";
                }
            }
            else
            {
                msg = "Карта не найдена.";
            }

            await client.SendTextMessageAsync(chatId, msg);
        }
    }
}