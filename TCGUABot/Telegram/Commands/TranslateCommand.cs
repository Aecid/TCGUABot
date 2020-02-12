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
    public class TranslateCommand : Command
    {
        public override string Name => "/translate ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;
            try
            {
                await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadPhoto);
            }
            catch { }

            var textToTranslate = message.Text.Replace("/translate ", "").Trim();

            var translateRequest = (HttpWebRequest)WebRequest.Create("https://translate.googleapis.com/translate_a/single?client=gtx&sl=auto&tl=ru&hl=en-US&dt=t&dt=bd&dj=1&source=input&tk=684737.684737&q=" + textToTranslate);

            WebResponse response = translateRequest.GetResponse();

            string json;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                json = reader.ReadToEnd();
            }

            var jArray = JObject.Parse(json);
            var translation = jArray["sentences"][0]["trans"].Value<string>();
            translation += " <i>(translated from:" + jArray["src"] + ")</i>";

            try
            {
                await client.SendTextMessageAsync(chatId, translation, Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: message.MessageId);
            }
            catch
            {

            }
        }
    }
}