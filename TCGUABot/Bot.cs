using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using TCGUABot.Models.Commands;
using TCGUABot.Models.Handlers;
using Telegram.Bot;

namespace TCGUABot
{
    public static class Bot
    {
        private static TelegramBotClient client;
        private static List<Command> commandsList;
        private static List<Handler> callbackHandlers;
        public static IReadOnlyList<Command> Commands { get => commandsList.AsReadOnly(); }
        public static IReadOnlyList<Handler> Handlers { get => callbackHandlers.AsReadOnly(); }

        public static async Task<TelegramBotClient> Get()
        {
            if (client != null)
            {
                return client;
            }

            commandsList = new List<Command>();
            callbackHandlers = new List<Handler>();

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
                .Where(t => t.Namespace != null && t.Namespace.StartsWith("TCGUABot.Models.Handlers"));

            Console.WriteLine("Initializing handlers:");

            foreach (var t in types)
            {
                if (t.Name.Equals("Handler") || !t.Name.EndsWith("Handler")) continue;
                Console.WriteLine(t.Name);
                var handler = Activator.CreateInstance(t);
                callbackHandlers.Add((Handler)handler);
            }

            client = new TelegramBotClient(Settings.ApiKey);
            var hook = string.Format(Settings.Url, "Update/Webhook");
            Console.WriteLine(hook);
            await client.SetWebhookAsync(hook);

            return client;
        }
    }
}
