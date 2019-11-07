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
    public class CatCommandStub : Command
    {
        public override string Name => "/randomCat";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {

            var chatId = message.Chat.Id;

            await client.SendTextMessageAsync(chatId, "Больше никаких котиков.\r\nАдминистрация этого чата ненавидит котиков.\r\n\r\nКотоненавистники.");
        }
    }
}