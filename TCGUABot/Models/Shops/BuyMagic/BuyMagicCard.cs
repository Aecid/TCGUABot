using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models.Shops.BuyMagic
{
    public class BuyMagicCard
    {
        public string name { get; set; }
        public float priceUah { get; set; }
        public int quantity { get; set; }
        public string set { get; set; }
        public string url { get; set; }

        public int quantityFoil { get; set; }
        public float priceUahFoil { get; set; }
    }
}
