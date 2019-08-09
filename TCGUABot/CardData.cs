using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TCGUABot.Models;

namespace TCGUABot
{
    public class CardData
    {
        private static CardData instance = null;
        private static readonly object padlock = new object();
        public List<Set> Sets = new List<Set>();
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
                //HttpClient client = new HttpClient();

                //using (Stream s = client.GetStreamAsync("https://mtgjson.com/json/AllSets.json").Result)
                //using (StreamReader sr = new StreamReader(s))
                //using (JsonReader reader = new JsonTextReader(sr))
                //{
                //    JsonSerializer serializer = new JsonSerializer();

                //    // read the json from a stream
                //    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                //    Sets = serializer.Deserialize<Dictionary<string, Set>>(reader);
                //}
                string filename = "AllSets.json";
                var json = ApiCall("https://mtgjson.com/json/SetList.json");
                var SetCodes = new List<string>();
                var jsonSetCodes = JsonConvert.DeserializeObject<dynamic>(json);
                foreach (var setCode in jsonSetCodes)
                {
                    SetCodes.Add(setCode.code.ToString());
                }

                if (DownloadAllCards(filename))
                {
                    using (var stream = File.OpenRead(filename))
                    using (StreamReader streamReader = new StreamReader(stream))
                    using (JsonTextReader reader = new JsonTextReader(streamReader))
                    {
                        reader.SupportMultipleContent = true;

                        var serializer = new JsonSerializer();
                        while (reader.Read())
                        {
                            if (reader.TokenType == JsonToken.StartObject && SetCodes.Contains(reader.Path))
                            {
                                var c = serializer.Deserialize<Set>(reader);
                                Sets.Add(c);
                            }
                        }
                    }
                }
                else
                {
                    throw new Exception("DOWNLOAD WAS NOT SUCCESS MWAHAHAHA YOU MEATBAG");
                }
                ////var json = GetModernCards();
                ////Sets = JsonConvert.DeserializeObject<Dictionary<string, Set>>(json);
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

        public static bool DownloadJson(string url, string filename)
        {
            if (!File.Exists(filename))
            {
                try
                {
                    var request = (HttpWebRequest)WebRequest.Create(url);

                    request.Method = "GET";
                    request.AutomaticDecompression = DecompressionMethods.Deflate | DecompressionMethods.GZip;

                    var content = string.Empty;

                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        using (var outputStream = File.OpenWrite(filename))
                        using (var stream = response.GetResponseStream())
                        {
                            using (var sr = new StreamReader(stream))
                            {
                                stream.CopyTo(outputStream);
                            }
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message + ":: \r\n" + e.InnerException.Message ?? "");
                    return false;
                }
            }
            else
                return true;
        }

        public string GetAllCards()
        {
            return ApiCall("https://mtgjson.com/json/AllSets.json");
        }

        public bool DownloadAllCards(string filename)
        {
            return DownloadJson("https://mtgjson.com/json/AllSets.json", filename);
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
