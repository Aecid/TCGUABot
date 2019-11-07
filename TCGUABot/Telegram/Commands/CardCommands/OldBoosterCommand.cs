using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TCGUABot.Data;
using TCGUABot.Resources;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace TCGUABot.Models.Commands
{
    public class OldBoosterCommand : Command
    {
        public override string Name => "/oldbooster";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            string text = string.Empty;
            string setName = string.Empty;
            text = message.Text.Replace("/booster ", "");

            var set = CardData.Instance.Sets.FirstOrDefault(z => z.code == text);
            var cards = new List<Card>();
            if (set == null)
            {
                try
                {
                    await client.SendTextMessageAsync(message.Chat.Id, "Wrong set. Usage: /booster set", replyToMessageId: message.MessageId);

                }
                catch { }
                return;
            }
            else
            {
                try
                {

                    cards = set.cards.Where(z => int.Parse(z.number) <= set.baseSetSize).ToList();
                }
                catch
                {
                    try
                    {
                        await client.SendTextMessageAsync(message.Chat.Id, "Set contains weird cards. Try another one.", replyToMessageId: message.MessageId);

                    }
                    catch { }
                }
            }

            var cardList = new List<Card>();
            var commons = new List<Card>();
            var uncommons = new List<Card>();
            var rares = new List<Card>();

            Random rnd = new Random();

            var basicLand = new Card();

            int roll = rnd.Next(0, 5);
            switch (roll)
            {
                case 1:
                    basicLand = cards.FirstOrDefault(z => z.name == "Plains");
                    break;
                case 2:
                    basicLand = cards.FirstOrDefault(z => z.name == "Forest");
                    break;
                case 3:
                    basicLand = cards.FirstOrDefault(z => z.name == "Mountain");
                    break;
                case 4:
                    basicLand = cards.FirstOrDefault(z => z.name == "Swamp");
                    break;
                case 5:
                    basicLand = cards.FirstOrDefault(z => z.name == "Island");
                    break;
                default:
                    basicLand = cards.FirstOrDefault(z => z.name == "Island");
                    break;
            }

            cardList.Add(basicLand);

            var blackCards = cards.Where(c => c.colorIdentity.Contains("B") && c.rarity.Equals("common") && !c.type.Contains("Basic Land")).ToList();
            var redCards = cards.Where(c => c.colorIdentity.Contains("R") && c.rarity.Equals("common") && !c.type.Contains("Basic Land")).ToList();
            var whiteCards = cards.Where(c => c.colorIdentity.Contains("W") && c.rarity.Equals("common") && !c.type.Contains("Basic Land")).ToList();
            var greenCards = cards.Where(c => c.colorIdentity.Contains("G") && c.rarity.Equals("common") && !c.type.Contains("Basic Land")).ToList();
            var blueCards = cards.Where(c => c.colorIdentity.Contains("U") && c.rarity.Equals("common") && !c.type.Contains("Basic Land")).ToList();

            var commonB = blackCards[rnd.Next(blackCards.Count())];
            cards.Remove(commonB);
            var commonR = redCards[rnd.Next(redCards.Count())];
            cards.Remove(commonR);
            var commonW = whiteCards[rnd.Next(whiteCards.Count())];
            cards.Remove(commonW);
            var commonG = greenCards[rnd.Next(greenCards.Count())];
            cards.Remove(commonG);
            var commonU = blueCards[rnd.Next(blueCards.Count())];
            cards.Remove(commonU);

            commons.Add(commonB);
            commons.Add(commonR);
            commons.Add(commonW);
            commons.Add(commonG);
            commons.Add(commonU);

            var commonCards = cards.Where(c => c.rarity.Equals("common") && !c.type.Contains("Basic Land")).ToList();
            var uncommonCards = cards.Where(c => c.rarity.Equals("uncommon") && !c.type.Contains("Basic Land")).ToList();
            var rareCards = cards.Where(c => (c.rarity.Equals("rare") || c.rarity.Equals("mythic")) && !c.type.Contains("Basic Land")).ToList();

            var count = commonCards.Count();
            for (int i = 0; i < 5; i++)
            {
                var cardToAdd = commonCards[rnd.Next(count)];
                commons.Add(cardToAdd);
                commonCards.Remove(cardToAdd);
            }

            count = uncommonCards.Count();
            for (int i = 0; i < 3; i++)
            {
                var cardToAdd = uncommonCards[rnd.Next(count)];
                uncommons.Add(cardToAdd);
                uncommonCards.Remove(cardToAdd);
            }

            count = rareCards.Count();
                rares.Add(rareCards[rnd.Next(count)]);

            cardList.AddRange(ShuffleList(commons));
            cardList.AddRange(uncommons);
            cardList.AddRange(rares);


            var chatId = message.Chat.Id;

            List<IAlbumInputMedia> media = new List<IAlbumInputMedia>();
            foreach (var card in cardList)
            {
                string url = card.multiverseId == 0 && card.tcgplayerProductId > 0 ? CardData.GetTcgPlayerImage(card.tcgplayerProductId) : "https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + card.multiverseId + "&type=card";

                var imp = new InputMediaPhoto(new InputMedia(url))
                {
                    Caption = card.name
                };

                media.Add(imp);
            }

            foreach (var z in media)
            {
                Console.WriteLine(z.Caption);
            }

            if (media.Count > 0 && media.Count <= 15)
            {
                var chunkedList = ChunkBy(media, 9);

                foreach (var list in chunkedList)
                {
                    await Task.Delay(500);
                    await client.SendMediaGroupAsync(list, chatId);
                }
            }
        }

        private static List<Bitmap> ConvertUrlsToBitmaps(List<string> imageUrls, WebProxy proxy = null)
        {
            List<Bitmap> bitmapList = new List<Bitmap>();
            // Loop URLs
            foreach (string imgUrl in imageUrls)
            {
                try
                {
                    WebClient wc = new WebClient();
                    // If proxy setting then set
                    if (proxy != null)
                        wc.Proxy = proxy;
                    // Download image
                    byte[] bytes = wc.DownloadData(imgUrl);
                    MemoryStream ms = new MemoryStream(bytes);
                    Image img = Image.FromStream(ms);
                    bitmapList.Add((Bitmap)img);
                }
                catch (Exception ex)
                {
                    Console.Write(ex.Message);
                }
            }

            return bitmapList;
        }

        private List<List<T>> ChunkBy<T>(List<T> source, int chunkSize)
        {
            return source
                .Select((x, i) => new { Index = i, Value = x })
                .GroupBy(x => x.Index / chunkSize)
                .Select(x => x.Select(v => v.Value).ToList())
                .ToList();
        }

        private List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }
    }
}