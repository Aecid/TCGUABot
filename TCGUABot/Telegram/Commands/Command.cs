﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public abstract class Command
    {
        public abstract string Name { get; }
        public abstract Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context);
        public bool StartsWith(string command)
        {
            return command.StartsWith(this.Name, StringComparison.InvariantCultureIgnoreCase);
        }
    }
}
