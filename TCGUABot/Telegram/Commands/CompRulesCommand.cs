using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TCGUABot.Models.Commands
{
    public class CompRulesCommand : Command
    {
        public override string Name => "/cr";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;
            var rule = string.Empty;

            if (message.Text.StartsWith("/cr "))
            {
                var text = message.Text.Replace("/cr ", "");
                rule = BotData.ComprehensiveRules.FirstOrDefault(r => r.StartsWith(text));
            } 
            else if(message.Text.StartsWith("/cr? "))
            {
                var text = message.Text.Replace("/cr? ", "");
                rule = BotData.ComprehensiveRules.FirstOrDefault(r => r.Contains(text));
            }
            else
            {
                rule = "Usage: <b>/cr {rule number}</b>";
            }

            try
            {
                if (rule == string.Empty) rule = "Rule not found.";
                await client.SendTextMessageAsync(chatId, rule, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
            }
            catch { }
        }
    }
}