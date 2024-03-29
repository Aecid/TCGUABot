﻿using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TCGUABot.Models;
using TCGUABot.Models.TcgPlayerModels;

namespace TCGUABot
{
    public class CardData
    {
        private static CardData instance = null;
        public static List<string> Names = new List<string>();
        private static readonly object padlock = new object();
        public readonly List<Set> Sets = new List<Set>();
        public static List<TcgPlayerGroup> TcgGroups { get; private set; }
        public string Version = string.Empty;
        public static string BearerToken { get; private set; }
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
            BearerToken = GetTcgplayerAccessToken();
            TcgGroups = GetTcgPlayerGroups();

            //var version = GetVersion();
            if (Sets.Count==0)
            {
                //HttpClient client = new HttpClient();

                //using (Stream s = client.GetStreamAsync("https://mtgjson.com/api/v5/AllSets.json").Result)
                //using (StreamReader sr = new StreamReader(s))
                //using (JsonReader reader = new JsonTextReader(sr))
                //{
                //    JsonSerializer serializer = new JsonSerializer();

                //    // read the json from a stream
                //    // json size doesn't matter because only a small piece is read at a time from the HTTP request
                //    Sets = serializer.Deserialize<Dictionary<string, Set>>(reader);
                //}
                string filename = "AllSets.json";
                var json = ApiCall("https://mtgjson.com/api/v5/SetList.json");
                var SetCodes = new List<string>();
                var jsonSetCodes = JsonConvert.DeserializeObject<dynamic>(json);
                foreach (var setCode in jsonSetCodes.data)
                {
                    try
                    {
                        SetCodes.Add(setCode.code.ToString());
                    }
                    catch { }
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
                            if (reader.TokenType == JsonToken.StartObject && SetCodes.Contains(reader.Path.Replace("data.","")))
                            {
                                var c = serializer.Deserialize<Set>(reader);
                                Sets.Add(c);
                            }
                            var zz = reader.Path;
                            if (SetCodes.Contains(reader.Path.Replace("data.", "")))
                            {
                                var z = reader.TokenType;
                                try
                                {
                                    var c = serializer.Deserialize<Set>(reader);
                                    Sets.Add(c);
                                }
                                catch { }
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

            foreach (var set in Sets)
            {
                foreach (var card in set.cards)
                {
                    if (!Names.Contains(card.name))
                    {
                        Names.Add(card.name);
                    }
                    if (!Names.Contains(card.ruName) && !string.IsNullOrEmpty(card.ruName))
                    {
                        Names.Add(card.ruName);
                    }
                }
            }
        }

        public static List<TcgPlayerProductDetails> TcgSearchByName(string name)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var result = new Dictionary<string, float>();
            var version = configuration.GetSection("TCGPlayer").GetSection("Version").Value;

            var url = "https://api.tcgplayer.com/" + version + "/catalog/categories/1/search";
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + BearerToken);

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                string json = "{\r\n    " +
                    "\"sort\": \"name\",\r\n    " +
                    "\"limit\": 10,\r\n    " +
                    "\"offset\": 0,\r\n    " +
                    "\"filters\": " +
                    "[\r\n    \t" +
                    "{ \"name\": \"ProductName\", " +
                    "\"values\": [ \"" + name + "\" ] }\r\n    ]    \r\n}";

                streamWriter.Write(json);
            }

            var content = string.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using var stream = response.GetResponseStream();
                using (var sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }

            List<string> results = new List<string>();

            var res = JsonConvert.DeserializeObject<dynamic>(content);
            if ((bool)res.success)
            {
                foreach (var singleRes in res.results)
                {
                    results.Add((string)singleRes);
                }
            }

            url = "https://api.tcgplayer.com/" + version + "/catalog/products/" + string.Join(",", results);
            var req = (HttpWebRequest)WebRequest.Create(url);

            req.Method = "GET";
            req.ContentType = "application/json";
            req.Headers.Add("Authorization", "Bearer " + BearerToken);

            content = string.Empty;

            using (var response = (HttpWebResponse)req.GetResponse())
            {
                using var stream = response.GetResponseStream();
                using (var sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }

            res = JsonConvert.DeserializeObject<TcgPlayerProductDetailsResponse>(content);

            if (res.success)
            {
                return res.results;
            }
            else
            {
                return null;
            }

        }


        public static List<TcgPlayerProductDetails> TcgGetGroupContentById(int groupId)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var result = new Dictionary<string, float>();
            var version = configuration.GetSection("TCGPlayer").GetSection("Version").Value;

            var cards = new List<TcgPlayerProductDetails>();
            int offset = 0;

            while (true)
            {
                var url = "https://api.tcgplayer.com/" + version + "/catalog/products?getExtendedFields=true&offset=" + offset+"&limit=100&groupId=" + groupId.ToString();
                var req = (HttpWebRequest)WebRequest.Create(url);

                req.Method = "GET";
                req.ContentType = "application/json";
                req.Headers.Add("Authorization", "Bearer " + BearerToken);

                var content = string.Empty;

                using (var response = (HttpWebResponse)req.GetResponse())
                {
                    using var stream = response.GetResponseStream();
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }

                var res = JsonConvert.DeserializeObject<TcgPlayerGroupProductsListResponse>(content);

                if (res.success)
                {
                    cards.AddRange(res.results.ToList());
                }
                else
                {
                    break;
                }

                if (res.totalItems > cards.Count)
                {
                    offset += 100;
                }
                else
                {
                    break;
                }
            }


            return cards;
        }



        public static string GetTcgplayerAccessToken()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var clientId = configuration.GetSection("TCGPlayer").GetSection("ClientId").Value;
            var clientSecret = configuration.GetSection("TCGPlayer").GetSection("ClientSecret").Value;

            var request = (HttpWebRequest)WebRequest.Create("https://api.tcgplayer.com/token");
            request.Method = "POST";
            request.ContentType = "application/json";
            string postData = "grant_type=client_credentials&client_id=" + clientId + "&client_secret=" + clientSecret;

            var encoding = new UTF8Encoding();
            byte[] byte1 = encoding.GetBytes(postData);

            // Set the content length of the string being posted.
            request.ContentLength = byte1.Length;

            Stream newStream = request.GetRequestStream();
            newStream.Write(byte1, 0, byte1.Length);

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

            var responseObj = JsonConvert.DeserializeObject<dynamic>(content);
            return responseObj.access_token;
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

        public static Dictionary<string, float> GetTcgPlayerPrices(int productKey)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var result = new Dictionary<string, float>();
            var version = configuration.GetSection("TCGPlayer").GetSection("Version").Value;

            var url = "https://api.tcgplayer.com/" + version + "/pricing/product/" + productKey.ToString();
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + BearerToken);

            var content = string.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using var stream = response.GetResponseStream();
                using (var sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }

            var res = JsonConvert.DeserializeObject<dynamic>(content);
            IEnumerable<dynamic> results = res.results;
            var pfoil = results.FirstOrDefault(za => za.subTypeName == "Foil");
            var pnormal = results.FirstOrDefault(za => za.subTypeName == "Normal");

            float priceNormal = 0;
            float priceFoil = 0;
            float marketPriceNormal = 0;
            float marketPriceFoil = 0;

            if (float.TryParse(pnormal.midPrice.ToString(), out priceNormal))
                result.Add("normal", priceNormal);
            else result.Add("normal", 0);

            if (float.TryParse(pfoil.midPrice.ToString(), out priceFoil))
                result.Add("foil", priceFoil);
            else result.Add("foil", 0);

            if (float.TryParse(pnormal.marketPrice.ToString(), out marketPriceNormal))
                result.Add("marketNormal", marketPriceNormal);
            else result.Add("marketNormal", 0);

            if (float.TryParse(pfoil.marketPrice.ToString(), out marketPriceFoil))
                result.Add("marketFoil", marketPriceFoil);
            else result.Add("marketFoil", 0);


            return result;
        }

        public static string GetTcgPlayerPriceTemp(int productKey)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var result = new Dictionary<string, float>();
            var version = configuration.GetSection("TCGPlayer").GetSection("Version").Value;

            var url = "https://api.tcgplayer.com/" + version + "/pricing/product/" + productKey.ToString();
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + BearerToken);

            var content = string.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using var stream = response.GetResponseStream();
                using (var sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }

            return content;
        }

        public static IEnumerable<dynamic> GetTcgPlayerPrices(List<int> productKeys)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var result = new Dictionary<string, float>();
            var version = configuration.GetSection("TCGPlayer").GetSection("Version").Value;

            var values = SplitList(productKeys, 50);
            //split by 50

            List<dynamic> results = new List<dynamic>();


            foreach (var keysChunk in values)
            {

                var keys = string.Join(",", keysChunk);

                var url = "https://api.tcgplayer.com/" + version + "/pricing/product/" + keys;
                var request = (HttpWebRequest)WebRequest.Create(url);

                request.Method = "GET";
                request.ContentType = "application/json";
                request.Headers.Add("Authorization", "Bearer " + BearerToken);

                var content = string.Empty;

                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using var stream = response.GetResponseStream();
                    using (var sr = new StreamReader(stream))
                    {
                        content = sr.ReadToEnd();
                    }
                }

                var res = JsonConvert.DeserializeObject<dynamic>(content);
                IEnumerable<dynamic> castRes = res.results;
                results.AddRange(castRes.ToList());
            }

            return results;
        }

        //http://api.tcgplayer.com/v1.17.0/catalog/products/137942,132438?getExtendedFields=true

        public static TcgPlayerProductDetails GetTcgProductDetails(int productKey)
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var result = new Dictionary<string, float>();
            var version = configuration.GetSection("TCGPlayer").GetSection("Version").Value;

            var url = "https://api.tcgplayer.com/" + version + "/catalog/products/" + productKey.ToString() + "?getExtendedFields=true";
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + BearerToken);

            var content = string.Empty;
            try
            {
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    using var stream = response.GetResponseStream();
                    using var sr = new StreamReader(stream);
                    content = sr.ReadToEnd();
                }

                var res = JsonConvert.DeserializeObject<TcgPlayerProductDetailsResponse>(content);
                return res.results[0];
            }
            catch
            {
                return null;
            }
        }

        public static string GetTcgPlayerImage(int productKey)
        {
            if (productKey == 0) return "";
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var result = new Dictionary<string, float>();
            var version = configuration.GetSection("TCGPlayer").GetSection("Version").Value;

            var url = "https://api.tcgplayer.com/" + version + "/catalog/products/" + productKey.ToString() + "/media";
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + BearerToken);

            var content = string.Empty;
            Console.WriteLine("Url TCGPlayer: " + url);
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

            try
            {
                var res = JsonConvert.DeserializeObject<dynamic>(content);
                IEnumerable<dynamic> results = res.results;
                IEnumerable<dynamic> firstResultContentList = results.First().contentList;
                var productUrl = firstResultContentList.First().url;

                return productUrl;
            }
            catch (Exception e)
            {
                throw new Exception("Something went wrong with tcgplayer image" + "\r\n" + e.Message + "\r\n" + e.InnerException.Message);
            }
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
            return ApiCall("https://mtgjson.com/api/v5/AllPrintings.json");
        }

        public bool DownloadAllCards(string filename)
        {
            return DownloadJson("https://mtgjson.com/api/v5/AllPrintings.json", filename);
        }

        public string GetModernCards()
        {
            return ApiCall("https://mtgjson.com/api/v5/Modern.json");
        }

        public string GetVersion()
        {
            return ApiCall("https://mtgjson.com/api/v5/version.json");
        }

        public List<TcgPlayerGroup> GetTcgPlayerGroups()
        {
            IConfigurationBuilder configurationBuilder = new ConfigurationBuilder();
            configurationBuilder.AddJsonFile("appsettings.json");
            IConfiguration configuration = configurationBuilder.Build();

            var result = new Dictionary<string, float>();
            var version = configuration.GetSection("TCGPlayer").GetSection("Version").Value;

            var url = "https://api.tcgplayer.com/" + version + "/catalog/categories/1/groups?limit=100";
            var request = (HttpWebRequest)WebRequest.Create(url);

            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers.Add("Authorization", "Bearer " + BearerToken);

            var content = string.Empty;

            using (var response = (HttpWebResponse)request.GetResponse())
            {
                using var stream = response.GetResponseStream();
                using (var sr = new StreamReader(stream))
                {
                    content = sr.ReadToEnd();
                }
            }

            var res = JsonConvert.DeserializeObject<TcgPlayerGroupsResponse>(content);

            var list = res.results;

            if (res.totalItems > 100)
            {
                var total = res.totalItems;
                var k = 1;
                while (true)
                {
                    total = total - 100;
                    var offset = k * 100;
                    var tempUrl = "https://api.tcgplayer.com/" + version + "/catalog/categories/1/groups?offset=" + offset + "&limit=100";
                    var tempRequest = (HttpWebRequest)WebRequest.Create(tempUrl);

                    tempRequest.Method = "GET";
                    tempRequest.ContentType = "application/json";
                    tempRequest.Headers.Add("Authorization", "Bearer " + BearerToken);

                    var tempContent = string.Empty;

                    using (var response = (HttpWebResponse)tempRequest.GetResponse())
                    {
                        using var stream = response.GetResponseStream();
                        using (var sr = new StreamReader(stream))
                        {
                            tempContent = sr.ReadToEnd();
                        }
                    }

                    var tempRes = JsonConvert.DeserializeObject<TcgPlayerGroupsResponse>(tempContent);

                    list.AddRange(tempRes.results);

                    k++;

                    if (total < 100) break;
                }
            }

            return list;
        }


        public static List<List<int>> SplitList(List<int> listToSplit, int nSize = 30)
        {
            var list = new List<List<int>>();

            for (int i = 0; i < listToSplit.Count; i += nSize)
            {
                list.Add(listToSplit.GetRange(i, Math.Min(nSize, listToSplit.Count - i)));
            }

            return list;
        }
    }
}
