using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TCGUABot.Models;

namespace TCGUABot.Controllers
{
    public class CardsController : ControllerBase
    {
        // GET: api/Cards
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpGet]
        public IEnumerable<dynamic> Stats()
        {
            dynamic stats = new ExpandoObject();
            var Sets = new List<dynamic>();
            foreach (var set in CardData.Instance.Sets)
            {
                Sets.Add(new { SetName = set.name, CardsCount = set.cards.Count });
            }

            //var result = JsonConvert.SerializeObject(stats);

            return Sets;
        }

        // GET: /Cards/Bolt
        [HttpGet("/Cards/{query}", Name = "SearchForCard")]
        public IEnumerable<Card> Get(string query)
        {
            var resultCards = new List<Card>();
            foreach (var set in CardData.Instance.Sets)
            {
                bool ruLang = false;
                List<Card> cards = new List<Card>();
                if (Regex.IsMatch(query, @"\p{IsCyrillic}"))
                {
                    ruLang = true;
                    cards = set.cards.Where(c => c.ruName.ToLower().Contains(query.ToLower())).ToList();
                }
                else
                {
                    ruLang = false;
                    cards = set.cards.Where(c => c.name.ToLower().Contains(query.ToLower())).ToList();
                }

                resultCards = resultCards.Concat(cards).ToList();
            }

            return resultCards;
        }

        [HttpGet("/Cards/id/{id}", Name = "SearchForCardByMuID")]
        public Card Get(int id)
        {
            return Helpers.CardSearch.GetCardByMultiverseId(id);
        }

        [HttpGet("/testCard", Name = "TestCard")]
        public string GetCardPrice()
        {
            var msg = string.Empty;
            var card = Helpers.CardSearch.GetCardByMultiverseId(430834);
            var prices = CardData.GetTcgPlayerPrices(card.tcgplayerProductId);
            if (prices["normal"] > 0)
                msg += "Цена: <b>$" + prices["normal"].ToString() + "</b>\r\n";
            if (prices["foil"] > 0)
                msg += "Цена фойлы: <b>$" + prices["foil"].ToString() + "</b>\r\n";
            if (prices["normal"] == 0 && prices["foil"] == 0)
                msg += "Цена: <i>Нет данных о цене</i>\r\n";

            return msg;
        }


        // POST: api/Cards
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT: api/Cards/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
