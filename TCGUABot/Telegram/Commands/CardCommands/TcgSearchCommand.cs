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
        public override string Name => "/tcgid";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var tUser = message.From;
            Helpers.TelegramUtil.AddUser(tUser, context);

            bool isVip = false;

            try
            {
                isVip = context.TelegramUsers.FirstOrDefault(t => t.Id == tUser.Id).IsVIP;
            }
            catch
            { }

            var chatId = message.Chat.Id;

            string text = "";

            if (message.Text.StartsWith("/tcgid "))
                text = message.Text.Replace("/tcgid ", "").Trim();

            if (message.Text.StartsWith("/tcgid_"))
                text = message.Text.Replace("/tcgid_", "").Trim();

            if (text.Contains("@tcgua_bot"))
                text = text.Replace("@tcgua_bot", "");

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
                    if (prices["marketNormal"] > 0)
                        price += Lang.Res(lang).marketPrice + ": <b>$" + prices["marketNormal"].ToString() + "</b>\r\n";
                    if (prices["foil"] > 0)
                        price += Lang.Res(lang).priceFoil + ": <b>$" + prices["foil"].ToString() + "</b>\r\n";
                    if (prices["marketFoil"] > 0)
                        price += Lang.Res(lang).marketPriceFoil + ": <b>$" + prices["marketFoil"].ToString() + "</b>\r\n";
                    if (prices["normal"] == 0 && prices["foil"] == 0 && prices["marketNormal"] == 0 && prices["marketFoil"] == 0)
                        price += Lang.Res(lang).price + ": <i>" + Lang.Res(lang).priceNoData + "</i>\r\n";

                }
                catch
                {
                }

                var legality = string.Empty;
                var legalCard = Helpers.CardSearch.GetCardByName(card.Name);
                if (legalCard != null)
                {
                    if (legalCard.legalities.Count > 0)
                    {
                        var notInterestingFormats = new List<string>()
                        {
                            "penny",
                            "future",
                            "duel",
                            "oldschool"
                        };

                        foreach (var legalItem in legalCard.legalities)
                        {
                            if (!notInterestingFormats.Contains(legalItem.Key))
                            {
                                var legalString = string.Empty;
                                if (legalItem.Value == "Legal") legalString = "<i>" + legalItem.Key + "</i>"; 
                                if (legalItem.Value == "Banned") legalString = "<i><s>" + legalItem.Key + "</s></i>";
                                if (legalItem.Value == "Restricted") legalString = legalItem.Key;
                                legality += legalString+" ";
                            }
                        }

                        if (legality.Trim() != "")
                        {
                            legality = "\r\n" + legality + "\r\n";
                        }
                    }
                }

                var msg = string.Empty;

                msg += nameEn + "\r\n" + set + legality + "\r\n" + price;


                WebRequest req;
                if (isVip) {
                    req = WebRequest.Create("https://api.scryfall.com/cards/tcgplayer/" + card.ProductId + "?format=image");
                }

                else
                {
                    req = WebRequest.Create(card.ImageUrl);
                }
                try
                {
                    using (Stream fileStream = req.GetResponse().GetResponseStream())
                    {
                        //var buttonsList = new List<InlineKeyboardButton>();
                        //buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTB", "trade|wtb|" + card.ProductId + "|" + card.Name.Replace("/", "\\/")));
                        //buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTS", "trade|wts|" + card.productId + "|" + card.name));
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
                        //buttonsList.Add(InlineKeyboardButton.WithCallbackData("WTS", "trade|wts|" + card.productId + "|" + card.name));
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