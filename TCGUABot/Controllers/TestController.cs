using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCGUABot.Data;
using TCGUABot.Models.Commands;
using TCGUABot.Models.InlineQueryHandler;
using Telegram.Bot.Types;

namespace TCGUABot.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly ApplicationDbContext context;

        public TestController(ApplicationDbContext _context)
        {
            context = _context;
        }

        [HttpGet("/Test/CardCommand/{cardName}", Name = "CardCommandCall")]
        public async Task<string> CardCommand(string cardName)
        {

            var msg = string.Empty;

            var command = new CardCommand();
            await command.Execute(new Message() { Text = "/c " + cardName, Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }

        [HttpGet("/Test/InlineQuery/{cardName}", Name = "InlineQueryCall")]
        public async Task<string> InlineQuery(string cardName)
        {

            var msg = string.Empty;

            var command = new InlineQueryHandler();
            await command.Execute(new InlineQuery() { Query = cardName }, await Bot.Get(), context);

            return msg;
        }

        [HttpGet("/Test/SettingsCommand", Name = "SettingsCommandCall")]
        public async Task<string> SettingsCommand(string cardName)
        {

            var msg = string.Empty;

            var command = new CardCommand();
            await command.Execute(new Message() { Text = "/c " + cardName, Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }

        [HttpGet("/Test/GuideCommand", Name = "GuideCommandCall")]
        public async Task<string> GuideCommand(string keyWord)
        {

            var msg = string.Empty;

            var command = new GuideCommand();
            await command.Execute(new Message() { Text = "/guide " + keyWord, Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }

        [HttpGet("/Test/FileCommand", Name = "FileCommandCall")]
        public async Task<string> FileCommand(string query)
        {

            var msg = string.Empty;

            var command = new FileCommand();
            await command.Execute(new Message() { Text = "/file" + query, Chat = new Chat() { Id = 186070199 }, From = new User() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }

        [HttpGet("/Test/Spoilers", Name="MythisSpoilersCall")]
        public ActionResult MS()
        {
            var z = Helpers.MythicSpoilerParsing.GetNewSpoilers(context);

            return Ok(z);
        }

        [HttpGet("/Test/QueryCommand/{query}", Name = "QueryCommandCall")]
        public async Task<string> QueryCommand(string query)
        {

            var msg = string.Empty;

            var command = new QueryCommand();
            await command.Execute(new Message() { Text = "/q "+query, Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }

        [HttpGet("/Test/TourneyCommand", Name = "TourneyCommandCall")]
        public async Task<string> TourneyCommand()
        {

            var msg = string.Empty;

            var command = new TourneyCommand();
            await command.Execute(new Message() { Text = "/tourney", Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }

        [HttpGet("/Test/BoosterCommand", Name = "BoosterCommandCall")]
        public async Task<string> BoosterCommand()
        {

            var msg = string.Empty;

            var command = new BoosterCommand();
            await command.Execute(new Message() { Text = "/booster NEM", Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }

        [HttpGet("/Test/GetProductDetailsById/{productId}", Name = "GetProductDetailsById")]
        public string GetProductDetailsById(int productId)
        {
            var z = CardData.GetTcgProductDetails(productId);

            var res = string.Empty;

            res += JsonConvert.SerializeObject(z, Formatting.Indented);

            return res;
        }

        [HttpGet("/Test/GetProductPriceById/{productId}", Name = "GetProductPriceById")]
        public string GetProductPriceById(int productId)
        {
            var z = CardData.GetTcgPlayerPriceTemp(productId);

            var res = string.Empty;

            res += JsonConvert.SerializeObject(z, Formatting.Indented);

            return res;
        }

        [HttpGet("/Test/GetGroupsByCategoryId/{productId}", Name = "GetGroupsByCategoryId")]
        public string GetGroupsByCategoryId(int productId)
        {
            var z = CardData.GetTcgProductDetails(productId);

            var res = string.Empty;

            res += JsonConvert.SerializeObject(z, Formatting.Indented);

            return res;
        }

        [HttpGet("/Test/ShowAllGroups", Name = "ShowAllGroups")]
        public string ShowAllGroups()
        {
            var z = CardData.TcgGroups;
            var res = string.Empty;

            foreach (var group in z)
            {
                res += JsonConvert.SerializeObject(group, Formatting.Indented);
                res += "\r\n_____________________\r\n";
            }

            return res;
        }

        [HttpGet("/Test/ShowGroupContentById/{groupId}", Name = "ShowGroupContentById")]
        public string ShowGroupContentById(int groupId)
        {
            var z = CardData.TcgGroups;
            var res = string.Empty;

            foreach (var group in z)
            {
                res += JsonConvert.SerializeObject(group, Formatting.Indented);
                res += "\r\n_____________________\r\n";
            }

            return res;
        }

        [HttpGet("/Test/Group/{groupId}", Name = "GetGroupContent")]
        public string GetGroupContentById(int groupId)
        {
            var z = CardData.TcgGetGroupContentById(groupId);
            var res = string.Empty;
            res += "Total cards: " + z.Count().ToString();
            res += "\r\n";
            res += JsonConvert.SerializeObject(z);
            return res;
        }




        [HttpGet("/Test/Tcg/{name}", Name = "SearchTcg")]
        public string SearchTcg(string name)
        {
            var z = CardData.TcgSearchByName(name);

            var res = string.Empty;

            foreach (dynamic a in z)
            {
                res += "\r\n" + (string)a.name;
                res += "\r\n" + CardData.TcgGroups.FirstOrDefault(z => z.groupId == (int)a.groupId).name;
            }

            return res;
        }

        [HttpGet("/Test/Pricelist/", Name = "Pricelist")]
        public string PriceList()
        {
            var plist = new List<int>();
            plist.Add(228724);
            plist.Add(228725);
            plist.Add(228726);
            plist.Add(228727);
            plist.Add(228728);
            plist.Add(36767);
        


            var z = CardData.GetTcgPlayerPrices(plist);

            var res = JsonConvert.SerializeObject(z);

            return res;
        }

        [HttpGet("/Test/Tcgid/{id}", Name = "SearchTcgById")]
        public async Task<string> SearchTcgById(string id)
        {
            var msg = string.Empty;

            var command = new TcgSearchCommand();
            await command.Execute(new Message() { Text = "/tcgid "+id, Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);
            return msg;
        }

        [HttpGet("/Test/wtb/{name}", Name = "wtbCommandCall")]
        public async Task<string> WtbCommand(string name)
        {
            var msg = string.Empty;

            var command = new WtbCommand();
            await command.Execute(new Message() { Text = "/wtb " + name, Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }

        [HttpGet("/Test/tc/{name}", Name = "testCardCommandCall")]
        public async Task<string> TCCommand(string name)
        {
            var msg = string.Empty;

            var command = new TestCardCommand();
            await command.Execute(new Message() { Text = "/tc " + name, Chat = new Chat() { Id = 186070199 } }, await Bot.Get(), context);

            return msg;
        }
    }
}