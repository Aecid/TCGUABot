using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    public class CompRulesCommand : Command
    {
        public override string Name => "/cr";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;
            var rule = string.Empty;
            var rules = new List<string>();
            var callBack = "^";
            var text = "";
            if (message.Text.Contains("@")) text = message.Text.Split("@")[0];
            else text = message.Text;

            if (text.StartsWith("/cr "))
            {
                text = text.Replace("/cr ", "");
                callBack = "^";
                rules = BotData.ComprehensiveRules.Where(r => r.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)).ToList();
                rule = Regex.Replace(rules[0], @"(?!^)(\d+\d+\d+\.?\d?)", m => string.Format(@"/cr_{0}", m.Value.Replace(".", "_")));
            }
            else if (text.StartsWith("/cr_"))
            {
                text = text.Replace("/cr_", "");
                if (text.EndsWith(".")) text = text.Substring(0, text.Length - 1);
                text = text.Replace("_", ".");
                callBack = "^";
                rules = BotData.ComprehensiveRules.Where(r => r.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)).ToList();
                rule = Regex.Replace(rules[0], @"(?!^)(\d+\d+\d+\.?\d?)", m => string.Format(@"/cr_{0}", m.Value.Replace(".", "_")));
            }
            else if(text.StartsWith("/cr? "))
            {
                text = text.Replace("/cr? ", "");
                callBack = "?";
                rules = BotData.ComprehensiveRules.Where(r => r.Contains(text, StringComparison.InvariantCultureIgnoreCase)).ToList();
                rule = Regex.Replace(rules[0], @"(?!^)(\d+\d+\d+\.?\d?)", m => string.Format(@"/cr_{0}", m.Value.Replace(".", "_")));
            }
            else
            {
                rule = "Usage: <b>/cr {rule number}</b>";
            }

            try
            {
                if (rule == string.Empty) rule = "Rule not found.";

                var keyboardList = new List<List<InlineKeyboardButton>>();
                var buttonList = new List<InlineKeyboardButton>();
                var index = 0;

                if (index < rules.Count - 1)
                {
                    buttonList.Add(InlineKeyboardButton.WithCallbackData("➡️", "cr|"+callBack+"|" + (index + 1) + "|" + text));

                    keyboardList.Add(buttonList);
                }
                await client.SendTextMessageAsync(chatId, rule, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId, replyMarkup: new InlineKeyboardMarkup(keyboardList));
            }
            catch { }
        }
    }
}