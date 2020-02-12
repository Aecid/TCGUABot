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
    public class TcgSearchCommand : Command
    {
        public override string Name => "/tcgid ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var tUser = message.From;
            Helpers.TelegramUtil.AddUser(tUser, context);

            var chatId = message.Chat.Id;

            var text = message.Text.Replace("/tcgid ", "").Trim();

            try
            {
                await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadPhoto);
            }
            catch
            {

            }

            //var card = CardData.GetTcgProductDetails(int.Parse(text));
            var card = context.Cards.FirstOrDefault(z => z.ProductId == int.Parse(text));

            string price = string.Empty;
            if (card != null)
            {
                var lang = context.TelegramChats.FirstOrDefault(f => f.Id == chatId)?.Language;
                lang = lang == null ? "ru" : lang;

                var nameEn = "<a href=\""+card.Url+"\">" + Lang.Res(lang).enFlag + " " + card.Name + "</a>";
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
                        price += Lang.Res(lang).price + ": <b>$" + prices["normal"].ToString() + "</b>\r\n";
                    if (prices["foil"] > 0)
                        price += Lang.Res(lang).priceFoil + ": <b>$" + prices["foil"].ToString() + "</b>\r\n";
                    if (prices["normal"] == 0 && prices["foil"] == 0)
                        price += Lang.Res(lang).price + ": <i>" + Lang.Res(lang).priceNoData + "</i>\r\n";

                }
                catch
                {
                }

                var msg = string.Empty;

                msg += nameEn + "\r\n" + set + "\r\n" + price;

                var req = WebRequest.Create(card.ImageUrl);
                try
                {
                    using (Stream fileStream = req.GetResponse().GetResponseStream())
                    {
                        var buttonsList = new List<InlineKeyboardButton>();
                        buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTB", "trade|wtb|" + card.ProductId + "|" + card.Name.Replace("/", "\\/")));
                        //buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTS", "trade|wts|" + card.productId + "|" + card.name));
                        var keyboard = new InlineKeyboardMarkup(buttonsList);

                        try
                        {
                            await client.SendPhotoAsync(chatId, new InputOnlineFile(fileStream), msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard);
                        }
                        catch { }
                    }
                }
                catch
                {
                        var buttonsList = new List<InlineKeyboardButton>();
                        buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTB", "trade|wtb|" + card.ProductId + "|" + card.Name));
                        //buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTS", "trade|wts|" + card.productId + "|" + card.name));
                        var keyboard = new InlineKeyboardMarkup(buttonsList);
                        msg += "\r\n" + card.ImageUrl;

                        try
                        {
                            await client.SendTextMessageAsync(chatId, msg, Telegram.Bot.Types.Enums.ParseMode.Html, replyMarkup: keyboard);
                        }
                        catch { }
                }
            }
        }
    }
}