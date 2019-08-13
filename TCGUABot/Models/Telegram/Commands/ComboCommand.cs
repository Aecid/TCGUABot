using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TCGUABot.Models.Commands
{
    public class ComboCommand : Command
    {
        public override string Name => "/combo ";

        public override async void Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            string text = string.Empty;
            string setName = string.Empty;
            text = message.Text.Replace("/combo ", "");
            var queryCards = text.Trim().Split("+");
            var msg = string.Empty;
            var chatId = message.Chat.Id;
            var ComboList = new List<Card>();
            Console.WriteLine("Cards in combo: " + queryCards.ToList().Count);

            foreach (var comboPiece in queryCards)
            {
                var cpName = comboPiece.Trim();
                Console.WriteLine("|" + comboPiece + "|" + cpName + "|");
                Card card;
                    card = Helpers.CardSearch.GetCardByName(cpName);

                if (card != null)
                {
                    Console.WriteLine("Card found");
                    ComboList.Add(card);
                }
                else
                {
                    Console.WriteLine("Not found");
                    msg += "❌Карта " + cpName + " была не найдена\r\n";
                }
            }

            List<IAlbumInputMedia> media = new List<IAlbumInputMedia>();
            foreach (var card in ComboList)
            {
                //var req = WebRequest.Create("https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card");

                //using (Stream fileStream = req.GetResponse().GetResponseStream())
                //{
                    var imp = new InputMediaPhoto(new InputMedia("https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card"))
                    {
                        Caption = card.name
                    };
                    media.Add(imp);
   //             }

 //               req.
            }

            foreach (var z in media)
            {
                Console.WriteLine(z.Caption);
            }

            if (media.Count > 0)
            {
                await client.SendMediaGroupAsync(media, chatId);
            }
            if (msg.Length > 5)
            {
                await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyToMessageId: message.MessageId);
            }

        }
    }
}