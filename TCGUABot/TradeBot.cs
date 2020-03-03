using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TCGUABot.Models.Commands;
using TCGUABot.Models.CallbackHandlers;
using Telegram.Bot;
using TCGUABot.Data;

namespace TCGUABot
{
    public static class TradeBot
    {
        private static TelegramBotClient client;
        private static List<Command> commandsList;
        private static List<CallbackHandler> callbackHandlers;
        public static IReadOnlyList<Command> Commands { get => commandsList.AsReadOnly(); }
        public static IReadOnlyList<CallbackHandler> CallbackHandlers { get => callbackHandlers.AsReadOnly(); }

        public static async Task<TelegramBotClient> Get()
        {
            if (client != null)
            {
                return client;
            }

            commandsList = new List<Command>();
            
            callbackHandlers = new List<CallbackHandler>();

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            client = new TelegramBotClient(configuration.GetSection("TelegramSettings").GetSection("TradeBotToken").Value);

            Console.WriteLine(configuration.GetSection("TelegramSettings").GetSection("TradeBotToken").Value);
            var hook = string.Format(configuration.GetSection("TelegramSettings").GetSection("TelegramWebHookHostBase").Value, "Trade/Webhook");
            Console.WriteLine(hook);
            await client.SetWebhookAsync(hook);

            return client;
        }
    }
}
