using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TCGUABot.Models.Commands
{
    public class CatCommand : Command
    {
        public override string Name => "/randomCat";

        public override async void Execute(Message message, TelegramBotClient client)
        {

            var chatId = message.Chat.Id;
            await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadPhoto);

            var catRequest = (HttpWebRequest)WebRequest.Create("https://aws.random.cat/meow");

            WebResponse response = catRequest.GetResponse();

            string json;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                json = reader.ReadToEnd();
            }

            var jArray = JObject.Parse(json);
            var fileUrl = jArray["file"].Value<string>();

            var req = WebRequest.Create(fileUrl);

            using (Stream fileStream = req.GetResponse().GetResponseStream())
            {
                if (fileUrl.EndsWith(".gif"))
                {
                    var msg = await client.SendVideoAsync(chatId, new InputOnlineFile(fileStream), supportsStreaming: true);

                }
                else
                {
                    await client.SendPhotoAsync(chatId, new InputOnlineFile(fileStream));
                }
            }
        }
    }
}