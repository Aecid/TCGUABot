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
    public class CardWitasCommand : Command
    {
        public override string Name => "/card ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var command = new CardCommand();
            var newMessage = message;
            newMessage.Text = newMessage.Text.Replace("/card ", "/c ");
            await command.Execute(newMessage, client, context);
        }
    }
}