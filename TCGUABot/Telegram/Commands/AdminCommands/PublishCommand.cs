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
    public class PublishCommand : Command
    {
        public override string Name => "/tpublish";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            if (message.From.Id == 186070199)
            {
                var chatId = message.Chat.Id;
                var text = message.Text.Replace("/tpublish", "");

                try
                {
                    var command = new TourneyCommand();
                    await command.Execute(new Message() { Text = "/tourney", Chat = new Chat() { Id = -1001175802200 } }, await Bot.Get(), context);
                }

                catch { }
            }

            if (message.From.Id == 305751207)
            {
                var chatId = message.Chat.Id;
                var text = message.Text.Replace("/tpublish", "");

                try
                {
                    var command = new TourneyCommand();
                    await command.Execute(new Message() { Text = "/tourney", Chat = new Chat() { Id = -1001135635683 } }, await Bot.Get(), context);
                }

                catch { }
            }
        }
    }
}