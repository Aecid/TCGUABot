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
    public class TestCorvinCommand : Command
    {
        public override string Name => "/testCorvin";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;
            float latitude = 46.485030f;
            float longitude = 30.737538f;

            await client.SendVenueAsync(chatId, latitude, longitude, "Паб \"Корвин\"", "Ланжероновская, 17А", "4c36138d3849c9285e68bbb1");
        }
    }
}