using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
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
    public class ComRulesPaginationCallbackHandler : CallbackHandler
    {
        public override string Name => "cr";

        public override async Task Execute(CallbackQuery query, TelegramBotClient client, ApplicationDbContext context)
        {

            Console.WriteLine("|||" + query.Data + "|||");
            var dataArray = query.Data.Split("|");
            var pattern = dataArray[1];
            var index = int.Parse(dataArray[2]);
            var text = dataArray[3];

            string rule = "";

            var rules = new List<string>();
            if (pattern == "?")
                rules = BotData.ComprehensiveRules.Where(r => r.Contains(text, StringComparison.InvariantCultureIgnoreCase)).ToList();
            if (pattern == "^")
                rules = BotData.ComprehensiveRules.Where(r => r.StartsWith(text, StringComparison.InvariantCultureIgnoreCase)).ToList();

            if (index >= 0 && index <= rules.Count - 1)
            {
                rule = Regex.Replace(rules[index], @"(?!^)(\d+\d+\d+\.?\d?)", m => string.Format(@"/cr_{0}", m.Value.Replace(".", "_")));
            }
            else rule = rules[0];

            var keyboardList = new List<List<InlineKeyboardButton>>();
            var buttonList = new List<InlineKeyboardButton>();

            if (index > 0)
            {
                buttonList.Add(InlineKeyboardButton.WithCallbackData("⬅️", "cr|" + pattern + "|" + (index - 1) + "|" + text));
            }
            if (index < rules.Count - 1)
            {
                buttonList.Add(InlineKeyboardButton.WithCallbackData("➡️", "cr|" + pattern + "|" + (index + 1) + "|" + text));
            }

            keyboardList.Add(buttonList);

            try
            {
                await client.EditMessageTextAsync(query.Message.Chat.Id, query.Message.MessageId, rule, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: new InlineKeyboardMarkup(keyboardList));
            }
            catch { }
        }
    }
}
