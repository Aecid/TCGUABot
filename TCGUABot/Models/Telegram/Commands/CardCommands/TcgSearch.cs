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

namespace TCGUABot.Models.Commands
{
    public class TcgSearch : Command
    {
        public override string Name => "/p_id ";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            var tUser = message.From;
            Helpers.TelegramUtil.AddUser(tUser, context);

            var chatId = message.Chat.Id;

            var text = message.Text.Replace("/p_id ", "").Trim();

            try
            {
                await client.SendChatActionAsync(chatId, Telegram.Bot.Types.Enums.ChatAction.UploadPhoto);
            }
            catch
            {

            }

            var card = CardData.GetTcgProductDetails(int.Parse(text));

            string price = string.Empty;
            if (card != null)
            {
                var lang = context.TelegramChats.FirstOrDefault(f => f.Id == chatId)?.Language;
                lang = lang == null ? "ru" : lang;

                var nameEn = "<b>" + Lang.Res(lang).enFlag + " " + card.name + "</b>";
                var set = "<i>(" + CardData.TcgGroups.FirstOrDefault(z => z.groupId == card.groupId).name + ")</i>";


                try
                {
                    var prices = CardData.GetTcgPlayerPrices(card.productId);
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

                var req = WebRequest.Create(card.imageUrl);
                using (Stream fileStream = req.GetResponse().GetResponseStream())
                {
                    try
                    {
                        await client.SendPhotoAsync(chatId, new InputOnlineFile(fileStream), msg, Telegram.Bot.Types.Enums.ParseMode.Html);
                    }
                    catch { }
                }
            }
        }
    }
}