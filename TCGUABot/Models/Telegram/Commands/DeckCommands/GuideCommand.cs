using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using TCGUABot.Controllers;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class GuideCommand : Command
    {
        public override string Name => "/guide";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var chatId = message.Chat.Id;
            var text = message.Text.Replace("/guide", "").Trim();
            var msg = string.Empty;
            var format = string.Empty;
            if (text.Contains("legacy", StringComparison.InvariantCultureIgnoreCase) || text.Contains("легаси", StringComparison.InvariantCultureIgnoreCase)) format = "Legacy";
            else if (text.Contains("modern", StringComparison.InvariantCultureIgnoreCase) || text.Contains("модерн", StringComparison.InvariantCultureIgnoreCase)) format = "Modern";
            else if (text.Contains("standard", StringComparison.InvariantCultureIgnoreCase) || text.Contains("стандарт", StringComparison.InvariantCultureIgnoreCase)) format = "Standard";

            text = text.Replace("legacy", "").Replace("легаси", "").Replace("modern", "").Replace("модерн", "").Replace("standard", "").Replace("стандарт", "").Trim();

            if ((chatId != -225805602) && (chatId != 186070199)) return;

            if (!string.IsNullOrEmpty(text))
            {
                var guide = context.DeckGuides.FirstOrDefault(dg => dg.Keywords.Contains(text, StringComparer.InvariantCultureIgnoreCase) && dg.Format == format);
                var url = guide?.Url;

                if (url == null)
                {
                    msg = "Wrong keyword?";
                }
                else
                {
                    msg = "<a href=\""+url+"\">" + format + " " + text + " guide (last upd.:" + guide.LastUpdated.ToString("MM/dd/yyyy") + ")</a>";
                }
            }
            else
            {
                var guides = context.DeckGuides.ToList();

                var guidesLegacy = guides?.Where(g => g.Format == "Legacy");
                var guidesModern = guides?.Where(g => g.Format == "Modern");
                var guidesStandard = guides?.Where(g => g.Format == "Standard");

                msg += "<b>Legacy:</b>\r\n";
                foreach (var guide in guidesLegacy)
                {
                    msg += "<a href=\"" + guide.Url + "\">" + guide.Keywords.First() + " (upd.: " + guide.LastUpdated.ToString("MM/dd/yyyy") + ") " + "</a>" + "\r\n";
                }
                msg += "\r\n<b>Modern:</b>\r\n";
                foreach (var guide in guidesModern)
                {
                    msg += "<a href=\"" + guide.Url + "\">" + guide.Keywords.First() + " (upd.: " + guide.LastUpdated.ToString("MM/dd/yyyy") + ") " + "</a>" + "\r\n";
                }
                msg += "\r\n<b>Standard:</b>\r\n";
                foreach (var guide in guidesStandard)
                {
                    msg += "<a href=\"" + guide.Url + "\">" + guide.Keywords.First() + " (upd.: " + guide.LastUpdated.ToString("MM/dd/yyyy") + ") " + "</a>" + "\r\n";
                }
            }

            if (!string.IsNullOrEmpty(msg)) await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: message.MessageId, disableWebPagePreview: true, disableNotification: true);
        }
    }
}