using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
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
