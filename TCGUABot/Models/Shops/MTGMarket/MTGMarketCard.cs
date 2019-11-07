using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TCGUABot.Models.Shops.MTGMarket
{
    public class MTGMarketCard
    {
        public long id { get; set; }
        public string type { get; set; }
        public string link { get; set; }
        public MTGMarketTitle title { get; set; }
        public MTGMarketExcerpt excerpt { get; set; }

        public string name
        {
            get
            {
                string noHTML = Regex.Replace(title.rendered, @"<[^>]+>|&nbsp;", "").Trim();
                string noHTMLNormalised = Regex.Replace(noHTML, @"\s{2,}", " ");
                return noHTMLNormalised;
            }
        }
        public string fullName
        {
            get
            {
                string noHTML = Regex.Replace(excerpt.rendered, @"<[^>]+>|&nbsp;", "").Trim();
                string noHTMLNormalised = Regex.Replace(noHTML, @"\s{2,}", " ");
                return noHTMLNormalised;
            }
        }

        public List<string> Data {
            get
            {
                //"Могильщик, English, Normal, Mint/NM, Commander (CMD), №86 (https://www.mtgmarket.com.ua/shop/mtg-cards/gravedigger-english-6/)"
                return fullName.Split(", ").ToList();
            } 
        }

        public string lang
        {
            get
            {
                var z = string.Empty;
                try
                {
                    z = Data[1];
                }
                catch
                {

                }
                return z;
            }
        }

        public string foil
        {
            get
            {
                var z = string.Empty;
                try
                {
                    z = Data[2];
                }
                catch
                {

                }
                return z;
            }
        }

        public string state
        {
            get
            {
                var z = string.Empty;
                try
                {
                    z = Data[3];
                }
                catch
                {

                }
                return z;
            }
        }

        public string set
        {
            get
            {
                var z = string.Empty;
                try
                {
                    z = Data[4];
                }
                catch
                {

                }
                return z;
            }
        }
    }
}

//        {
//"id": 573897,
//"date": "2019-03-24T17:38:32",
//"date_gmt": "2019-03-24T15:38:32",
//"guid": {
//"rendered": "https://www.mtgmarket.com.ua/shop/uncategorized/golgari-thug-english-2/"
//},
//"modified": "2019-10-16T01:18:59",
//"modified_gmt": "2019-10-15T22:18:59",
//"slug": "golgari-thug-english-2",
//"status": "publish",
//"type": "product",
//"link": "https://www.mtgmarket.com.ua/shop/mtg-cards/golgari-thug-english-2/",
//"title": {
//"rendered": "Golgari Thug (English) <strong></strong>"
//},
//"content": {
//"rendered": "<p>Головорез Голгари, Ravnica: City of Guilds, English, Normal, Stamped, MTG Cards, Black, Uncommon, Creature, Human, Warrior,</p>\n",
//"protected": false
//},
//"excerpt": {
//"rendered": "<p>Головорез Голгари, English, Normal, Stamped, Ravnica: City of Guilds (RAV), №87</p>\n",
//"protected": false

