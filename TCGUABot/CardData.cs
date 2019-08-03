using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using TCGUABot.Models;

namespace TCGUABot
{
    public class CardData
    {
        private static CardData instance = null;
        private static readonly object padlock = new object();
        public Dictionary<string, Set> Sets;
        public string Version = string.Empty;
        public static CardData Instance
        {
            get
            {
                lock (padlock)
                {
                    if (instance == null)
                    {
                        instance = new CardData();
                    }
                }

                return instance;
            }
        }

        public CardData()
        {
            var version = GetVersion();
            if (version != Version || Sets == null)
            {
                // var json = GetAllCards();
                var json = GetModernCards();
                Sets = JsonConvert.DeserializeObject<Dictionary<string, Set>>(json);
            }
        }

        public static CardData Initalize()
        {
            return CardData.Instance;
        }

        public static string ApiCall(string url)
        {
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

            var content = string.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using (var stream = response.GetResponseStream())
                {
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }
            }

            return content;
        }

        public string GetAllCards()
        {
            return ApiCall("https://mtgjson.com/json/AllSets.json");
        }

        public string GetModernCards()
        {
            return ApiCall("https://mtgjson.com/json/Modern.json");
        }

        public string GetVersion()
        {
            return ApiCall("https://mtgjson.com/json/version.json");
        }
    }
}
