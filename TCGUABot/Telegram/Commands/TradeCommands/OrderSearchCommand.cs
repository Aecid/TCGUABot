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
    public class OrderSearchCommand : Command
    {
        public override string Name => "/wtb_tcgid_";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var tUser = message.From;
            Helpers.TelegramUtil.AddUser(tUser, context);

            var chatId = message.Chat.Id;

            var text = message.Text.Replace("/wtb_tcgid_", "").Trim().Replace("@tcgua_bot", "").Replace("@mtg_trade_bot", "");

            try
            {
                await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadPhoto);
            }
            catch
            {

            }

            //var card = CardData.GetTcgProductDetails(int.Parse(text));

            var card = new Product();

            try
            {
                card = context.Cards.FirstOrDefault(z => z.ProductId == int.Parse(text));
            }
            catch
            {
                try
                {
                    await client.SendTextMessageAsync(chatId, "Error ID-10-t", Telegram.Bot.Types.Enums.ParseMode.Html/*, replyMarkup: keyboard*/);
                }
                catch { }
            }

            string price = string.Empty;
            if (card != null)
            {
                var nameEn = "<a href=\""+card.Url+"\">" + card.Name + "</a>";
                string set = "";

                try
                {
                    set = "<i>(" + CardData.TcgGroups.FirstOrDefault(z => z.groupId == card.GroupId).name + ") (" + CardData.TcgGroups.FirstOrDefault(z => z.groupId == card.GroupId).abbreviation + ")</i>";
                }
                catch { set = "<i>Unknown Set</i>"; }


                try
                {
                    var prices = CardData.GetTcgPlayerPrices(card.ProductId);
                    if (prices["normal"] > 0)
                        price += "non-foil: <b>$"+prices["normal"].ToString() + "</b>\r\n";
                    if (prices["foil"] > 0)
                        price += "foil <b>$" + prices["foil"].ToString() + "</b>\r\n";

                }
                catch
                {
                }

                var msg = string.Empty;

                msg += nameEn + "\r\n" + set + "\r\n" + price;


                var orders = context.Orders.Where(x => x.ProductId == int.Parse(text));

                if (orders.Count() > 0)
                {
                    var orderText = string.Empty;
                    foreach (var order in orders)
                    {
                        var orderPrice = order.Price > 0 ? order.Price.ToString()+" грн" : order.ExchangeRate;
                        orderText += "\r\n" +order.Quantity+ "шт., <a href=\"tg://user?id=" + order.PlayerTelegramId + "\">Продавец</a>, "+ orderPrice + ", " + order.Location;
                    }

                    msg += orderText;
                }
                else
                {
                    msg += "Nothing found";
                }

                var req = WebRequest.Create(card.ImageUrl);
                try
                {
                    using (Stream fileStream = req.GetResponse().GetResponseStream())
                    {
                        //var buttonsList = new List<InlineKeyboardButton>();
                        //buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTB", "trade|wtb|" + card.ProductId + "|" + card.Name.Replace("/", "\\/")));
                        ////buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTS", "trade|wts|" + card.productId + "|" + card.name));
                        //var keyboard = new InlineKeyboardMarkup(buttonsList);

                        try
                        {
                            await client.SendPhotoAsync(chatId, new InputOnlineFile(fileStream), msg, Telegram.Bot.Types.Enums.ParseMode.Html/*, replyMarkup: keyboard*/);
                        }
                        catch { }
                    }
                }
                catch
                {
                        //var buttonsList = new List<InlineKeyboardButton>();
                        //buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTB", "trade|wtb|" + card.ProductId + "|" + card.Name));
                        ////buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTS", "trade|wts|" + card.productId + "|" + card.name));
                        //var keyboard = new InlineKeyboardMarkup(buttonsList);
                        msg += "\r\n" + card.ImageUrl;

                        try
                        {
                            await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html/*, replyMarkup: keyboard*/);
                        }
                        catch { }
                }
            }
        }
    }
}