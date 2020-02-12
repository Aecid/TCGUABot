using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    //TODO switch from CardCommand to this
    public class CardCommandNew : Command
    {
        public override string Name => "/c ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var tUser = message.From;
            Helpers.TelegramUtil.AddUser(tUser, context);

            var chatId = message.Chat.Id;
            try
            {
                await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadPhoto);
            }
            catch
            {

            }
            string text = string.Empty;
            string setName = string.Empty;
            var originalMessage = message.Text.Substring(message.Text.IndexOf("/c "));
            if (originalMessage.Contains("(") && originalMessage.Contains(")"))
            {
                var match = Regex.Match(originalMessage, @"/c (.*)\((.*)\)");
                text = match.Groups[1].Value;
                setName = match.Groups[2].Value;
            }
            else
            {
                text = originalMessage.Replace("/c ", "");
            }

            var lang = context.TelegramChats.FirstOrDefault(f => f.Id == chatId)?.Language;
            lang = lang == null ? "ru" : lang;
            var msg = string.Empty;
            Product card = null;

            var nonSupplemental = CardData.TcgGroups.Where(z => z.isSupplemental = false);
            var supplemental = CardData.TcgGroups.Where(z => z.isSupplemental = true);

            card = context.Cards.FirstOrDefault(c => c.Name.ToLower() == text.ToLower());
            if (card == null) card = context.Cards.FirstOrDefault(c => c.Name.ToLower().StartsWith(text.ToLower()));
            if (card == null) card = context.Cards.FirstOrDefault(c => c.Name.ToLower().Contains(text.ToLower()));
            if (card == null) msg = "<b>❌" + Lang.Res(lang).cardNotFoundByRequest + " \"" + text + "\".</b>\r\n" + Lang.Res(lang).tryAtTcgua + ".";
            else
            {

            }
        }
    }
}