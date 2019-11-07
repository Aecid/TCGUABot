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
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    public class FileCommand : Command
    {
        public override string Name => "/file";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;

            var files = Directory.GetFiles("Files/").ToList();

            if (message.Text.Equals(Name))
            {
                try
                {
                    await client.SendTextMessageAsync(chatId, "Usage: <b>/file list</b>", Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
                }
                catch { }
            }
            else if (message.Text.StartsWith("/file list") && !message.Text.Contains("_"))
            {
                //var keyboardList = new List<List<InlineKeyboardButton>>();
                var msg = "Available files (" + files.Count() + "):";

                foreach (var file in files)
                {
                    msg += "\r\n/file_" + file.Substring(0, file.Length - 4).Replace("Files/", "");
                    //var buttonList = new List<InlineKeyboardButton>();
                    //buttonList.Add(InlineKeyboardButton.)
                    //keyboardList.Add(buttonList);
                }

                try
                {
                    await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, true, true, message.MessageId);
                }
                catch { }
            }
            else if (message.Text.StartsWith("/file_"))
            {
                try
                {
                    await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadDocument);
                }
                catch { }

                var fileName = message.Text.Replace("/file_", "").Replace("@tcgua_bot", "");

                var foundFile = files.FirstOrDefault(f => f.Contains(fileName + "."));
                if (foundFile != null)
                {
                    using (FileStream fs = new FileStream(foundFile, FileMode.Open, FileAccess.Read))
                    {
                        try
                        {
                            var inputFile = new InputOnlineFile(fs);
                            inputFile.FileName = foundFile.Replace("Files/", "");
                            await client.SendDocumentAsync(message.From.Id, inputFile, inputFile.FileName, Telegram.Bot.Types.Enums.ParseMode.Html, false);
                        }
                        catch { }
                    }
                }
            }

            //var keyboard = new InlineKeyboardMarkup(keyboardList);

        }
    }
}