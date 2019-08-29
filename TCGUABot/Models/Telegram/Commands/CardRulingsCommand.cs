using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class CardRulingsCommand : Command
    {
        public override string Name => "/rs";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var text = message.Text.Replace("/rs ", "");
            var msg = string.Empty;
            var chatId = message.Chat.Id;
            var card = Helpers.CardSearch.GetCardByName(text);

            if (card != null)
            {
                msg += "<b>🇺🇸" + card.name + "</b>\r\n";
                if (card.foreignData.Any(c => c.language.Equals("Russian"))) msg += "<b>🇷🇺" + card.ruName + "</b>\r\n";
                msg += "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card";

                if (card.rulings.Count > 0)
                {
                    msg += "\r\nРулинги: ";
                    foreach (var ruling in card.rulings)
                    {
                        msg += "\r\n📝<b>" + ruling.date + ":</b> <i>" + ruling.text + "</i>\r\n";
                    }
                }
                else
                {
                    msg += "\r\n<b>❌Рулинги не найдены</b>";
                }
            }
            else
            {
                msg = "<b>❌Карта не найдена.</b>";
            }

            if (msg.Length <= 4096)
            await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html);
            else
            {
                msg = msg.Replace("<b>", "").Replace("</b>", "").Replace("<i>", "").Replace("</i>", "");
                if (msg.Length <= 4096)
                {
                    await client.SendTextMessageAsync(chatId, msg);
                }
                else
                {
                    var msgs = SplitByChunks(msg, 4096);
                    foreach (var ms in msgs)
                    {
                        Console.WriteLine(ms);
                        await Task.Delay(500);
                        await client.SendTextMessageAsync(chatId, ms);
                    }
                }
            }
        }

        static IEnumerable<string> SplitByChunks(string str, int maxChunkSize)
        {
            for (int i = 0; i < str.Length; i += maxChunkSize)
                yield return str.Substring(i, Math.Min(maxChunkSize, str.Length - i));
        }
}