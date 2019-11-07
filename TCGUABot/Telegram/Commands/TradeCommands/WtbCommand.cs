using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using TCGUABot.Data;
using TCGUABot.Models.Shops.BuyMagic;
using TCGUABot.Models.Shops.MTGMarket;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace TCGUABot.Models.Commands
{
    public class WtbCommand : Command
    {
        public override string Name => "/wtb ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var chatId = message.Chat.Id;
            var cardName = message.Text.Replace("/wtb ", "");

            bool shopBuyMagic = true;
            bool shopMtgMarket = true;
            bool shopTcgPlayer = false;

            var msg = string.Empty;

            msg += "Найдено:";

            //buymagic_start
            if (shopBuyMagic)
            {
                //var idx = cardName.IndexOf("/");
                //var cardNameBM = cardName;
                //if (idx != -1) cardNameBM = cardName.Remove(idx).Trim();

                var catRequest = (HttpWebRequest)WebRequest.Create("http://www.buymagic.com.ua/card/api/?card_name=" + cardName);

                WebResponse response = catRequest.GetResponse();

                string json;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    json = reader.ReadToEnd();
                }

                var index = json.IndexOf("[");
                if (index != -1) json = json.Remove(0, index);

                var buyMagicSearchResult = new List<BuyMagicCard>();
                try
                {
                    buyMagicSearchResult = JsonConvert.DeserializeObject<List<BuyMagicCard>>(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                msg += "\r\n🏬<a href=\"http://www.buymagic.com.ua\">BuyMagic.com.ua</a>";
                if (buyMagicSearchResult.Count > 0)
                {
                    foreach (var result in buyMagicSearchResult)
                    {
                        if (result.quantity > 0 && result.priceUah > 0)
                            msg += "\r\n  🔎<a href=\"" + System.Text.RegularExpressions.Regex.Unescape(result.url) + "\">" + result.name + "</a> (" + result.set + ") - [" + result.quantity + "шт] - <i>" + result.priceUah + "грн</i>";
                        if (result.quantityFoil > 0 && result.priceUahFoil > 0)
                            msg += "\r\n  🔎<a href=\"" + System.Text.RegularExpressions.Regex.Unescape(result.url) + "\">" + result.name + "</a> (" + result.set + ") <b>(Foil)</b> - [" + result.quantityFoil + "шт] - <i>" + result.priceUahFoil + "грн</i>";
                    }
                }
                else
                {
                    msg += "\r\n❌0 карт по фильтру \"" + cardName + "\"";
                }
            }
            //buymagic_end

            //mtgmarket
            if (shopMtgMarket)
            {
                //var idx = cardName.IndexOf("/");
                //var cardNameBM = cardName;
                //if (idx != -1) cardNameBM = cardName.Remove(idx).Trim();

                var catRequest = (HttpWebRequest)WebRequest.Create("https://www.mtgmarket.com.ua/wp-json/wp/v2/product?search=" + HttpUtility.UrlEncode(cardName));

                WebResponse response = catRequest.GetResponse();

                string json;
                using (Stream responseStream = response.GetResponseStream())
                {
                    StreamReader reader = new StreamReader(responseStream, Encoding.UTF8);
                    json = reader.ReadToEnd();
                }

                Console.WriteLine(json);

                var buyMagicSearchResult = new List<MTGMarketCard>();
                try
                {
                    buyMagicSearchResult = JsonConvert.DeserializeObject<List<MTGMarketCard>>(json);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                msg += "\r\n\r\n🏬<a href=\"https://www.mtgmarket.com.ua\">MTGMarket</a>";
                if (buyMagicSearchResult.Count > 0)
                {
                    foreach (var result in buyMagicSearchResult)
                    {
                        msg += "\r\n  🔎<a href=\"" + System.Text.RegularExpressions.Regex.Unescape(result.link) + "\">" + result.name + "</a>";
                    }
                }
                else
                {
                    msg += "\r\n❌0 карт по фильтру \"" + cardName + "\"";
                }
            }
            //mtgmarket_end


            //tcgplayer
            if (shopTcgPlayer)
            {
                var cards = CardData.TcgSearchByName(cardName);
                msg += "\r\n\r\n🏬<a href=\"https://www.tcgplayer.com\">TCGPlayer.com</a>";

                try
                {
                    if (cards.Count > 0)
                    {
                        foreach (var card in cards)
                        {
                            msg += "\r\n  🔎<a href=\"" + card.url + "\">" + card.name + "</a> - (" + CardData.TcgGroups.FirstOrDefault(z => z.groupId == card.groupId).name + ")";
                        }
                    }
                    else
                    {
                        msg += "\r\n❌Нет данных по запросу \"" + cardName + "\"";
                    }
                }
                catch
                {
                    msg += "\r\n❌Нет данных по запросу \"" + cardName + "\"";
                }
            }
            //tcgPlayer end


            try
            {
                await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, true, false, message.MessageId);
            }
            catch { }
        }
    }
}