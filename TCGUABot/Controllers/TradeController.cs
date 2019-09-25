using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TCGUABot.Data;

namespace TCGUABot.Controllers
{
    public class TradeController : Controller
    {
        ApplicationDbContext context { get; set; }
        public TradeController(ApplicationDbContext context)
        {
            this.context = context;
        }

        [HttpGet]
        public JsonResult GetTradeCard([FromQuery] string productId)
        {
            var cards = context.TradingCards.Where(tc => tc.ProductId.ToString() == productId);
            return Json(cards);
        }

        [HttpGet]
        public JsonResult WTB([FromQuery] string id)
        {
            dynamic result = new ExpandoObject();
            return Json(result);
        }
    }
}