using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TCGUABot.Models.Commands;
using TCGUABot.Models.CallbackHandlers;
using Telegram.Bot;

namespace TCGUABot
{
    public static class Bot
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

            var types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("TCGUABot.Models.Commands"));

            Console.WriteLine("Initializing commands:");

            foreach (var t in types)
            {
                if (t.Name.Equals("Command") || !t.Name.EndsWith("Command")) continue;
                Console.WriteLine(t.Name);
                var command = Activator.CreateInstance(t);
                commandsList.Add((Command)command);
            }

            types = Assembly
                .GetExecutingAssembly()
                .GetTypes()
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("TCGUABot.Models.CallbackHandlers"));

            Console.WriteLine("Initializing handlers:");

            foreach (var t in types)
            {
                if (t.Name.Equals("CallbackHandler") || !t.Name.EndsWith("CallbackHandler")) continue;
                Console.WriteLine(t.Name);
                var handler = Activator.CreateInstance(t);
                callbackHandlers.Add((CallbackHandler)handler);
            }

            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

           
            client = new TelegramBotClient(configuration.GetSection("TelegramSettings").GetSection("TelegramBotToken").Value);
            Console.WriteLine(configuration.GetSection("TelegramSettings").GetSection("TelegramBotToken").Value);
            var hook = string.Format(configuration.GetSection("TelegramSettings").GetSection("TelegramWebHookHostBase").Value, "Update/Webhook");
            Console.WriteLine(hook);
            await client.SetWebhookAsync(hook);

            return client;
        }
    }
}
