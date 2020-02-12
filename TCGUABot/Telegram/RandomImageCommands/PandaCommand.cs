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
    public class PandaCommand : Command
    {
        public override string Name => "/randomPanda";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;
            try
            {
                await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadPhoto);
            }
            catch { }

            var catRequest = (HttpWebRequest)WebRequest.Create("https://some-random-api.ml/img/panda");

            WebResponse response = catRequest.GetResponse();

            string json;
            using (Stream responseStream = response.GetResponseStream())
            {
                StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                json = reader.ReadToEnd();
            }

            var jArray = JObject.Parse(json);
            var fileUrl = jArray["link"].Value<string>();

            var req = WebRequest.Create(fileUrl);

            using (Stream fileStream = req.GetResponse().GetResponseStream())
            {
                if (fileUrl.EndsWith(".gif"))
                {
                    try
                    {
                        var msg = await client.SendVideoAsync(chatId, new InputOnlineFile(fileStream), supportsStreaming: true);
                    }
                    catch { }

                }
                else
                {
                    try
                    {
                        await client.SendPhotoAsync(chatId, new InputOnlineFile(fileStream));
                    }
                    catch { }
                }
            }
        }
    }
}