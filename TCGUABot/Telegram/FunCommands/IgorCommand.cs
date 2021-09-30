using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace TCGUABot.Models.Commands
{
    public class IgorCommand : Command
    {
        public override string Name => "/igor";

        public override async Task Execute(Message message, TelegramBotClient client, ApplicationDbContext context)
        {
            List<string> urls = new List<string>();
            urls.Add("https://namedb.ru/assets/uploads/2016/04/igor.jpg");
            urls.Add("https://cs.pikabu.ru/post_img/2013/03/13/11/1363195357_2039606812.jpg");
            urls.Add("https://www.meme-arsenal.com/memes/e107d5ca959bec893a954c77bd12fd7c.jpg");
            urls.Add("https://s.ura.news/760/images/news/upload/news/407/798/1052407798/217715598e8c6c59d86c21d491005816_250x0_760.491.0.0.jpg");
            urls.Add("https://miasskiy.ru/wp-content/uploads/2019/11/1476714704173034516.png");
            urls.Add("https://static.hdrezka.ac/i/2014/12/19/f49b29ca4d8d5dq77k59k.jpg");
            urls.Add("https://studio21.ru/wp-content/uploads/2019/05/ig-720x720.jpg");
            urls.Add("https://static.wikia.nocookie.net/ultimatepopculture/images/3/3a/Igor_Young_Frankenstein.jpeg");
            urls.Add("https://www.igorgorgonzola.com/img/prodotti/igorcreme/Gorgonzola-Igor-Creme.png");



            var chatId = message.Chat.Id;
            var tUser = message.From;
            try
            {
                int r = Bot.rnd.Next(urls.Count);

                await client.SendTextMessageAsync(chatId, urls[r]);
            }
            catch
            {

            }
        }
    }
}