using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using TCGUABot.Data.Models;

namespace TCGUABot.Models
{
    public class ImportDeck
    {
        public ApplicationUser Owner;
        public List<ImportCard> MainDeck;
        public List<ImportCard> SideBoard;

        public override string ToString()
        {
            var z = this.MainDeck.GroupBy(x => x.name);

            var sortedDeck = new ImportDeck();
            sortedDeck.MainDeck = new List<ImportCard>();
            sortedDeck.SideBoard = new List<ImportCard>();


            //Next block doesn't sort really, it just unites card with same name but different MultiverseId
            foreach (var card in this.MainDeck)
            {
                foreach (var compareCard in this.MainDeck)
                {
                    if (card != compareCard)
                    {
                        if (card.name.Equals(compareCard.name) && !card.set.Equals(compareCard.set))
                        {
                            card.count += compareCard.count;
                        }
                    }
                }

                if (!sortedDeck.MainDeck.Any(c => c.name.Equals(card.name)))
                {
                    sortedDeck.MainDeck.Add(card);
                }
            }

            foreach (var card in this.SideBoard)
            {
                foreach (var compareCard in this.SideBoard)
                {
                    if (card != compareCard)
                    {
                        if (card.name.Equals(compareCard.name) && !card.set.Equals(compareCard.set))
                        {
                            card.count += compareCard.count;
                        }
                    }
                }

                if (!sortedDeck.SideBoard.Any(c => c.name.Equals(card.name)))
                {
                    sortedDeck.SideBoard.Add(card);
                }
            }




            List<string> deckList = new List<string>();
            foreach (var importCard in sortedDeck.MainDeck)
            {
                var set = importCard.set?.Replace("(", "").Replace(")", "").Replace("DAR", "DOM");
                if (set == "NONE")
                {
                    deckList.Add(importCard.count + " " + importCard.name);
                }
                else if (!string.IsNullOrEmpty(set))
                {
                    Card card = new Card();
                    if (importCard.multiverseId > 0) card = CardData.Instance.Sets.FirstOrDefault(s => s.cards.Any(d => d.multiverseId == importCard.multiverseId)).cards.FirstOrDefault(c => c.multiverseId == importCard.multiverseId);
                    if (importCard.multiverseId == 0 && importCard.tcgPlayerProductId > 0) card = CardData.Instance.Sets.FirstOrDefault(s => s.cards.Any(d => d.tcgplayerProductId == importCard.tcgPlayerProductId)).cards.FirstOrDefault(c => c.tcgplayerProductId == importCard.tcgPlayerProductId);

                    if (card == null) card = CardData.Instance.Sets.FirstOrDefault(s => s.code.Equals(set) && s.type != "promo" && s.type != "funny" && s.type != "box").cards.FirstOrDefault(c => c.number == importCard.collectorNumber.ToString());
                    if (card == null) break;
                    if (card.name.Equals(importCard.name))
                    {
                        deckList.Add(importCard.count + " " + card.name);
                    }
                }
                else
                {
                    var card = Helpers.CardSearch.GetCardByName(importCard.name);
                    deckList.Add(importCard.count + " " + card.name);
                }
            }

            deckList.Add("\r\n");

            foreach (var importCard in this.SideBoard)
            {
                var set = importCard.set?.Replace("(", "").Replace(")", "").Replace("DAR", "DOM");
                if (!string.IsNullOrEmpty(set))
                {
                    Card card = new Card();
                    if (importCard.multiverseId > 0) card = CardData.Instance.Sets.FirstOrDefault(s => s.cards.Any(d => d.multiverseId == importCard.multiverseId)).cards.FirstOrDefault(c => c.multiverseId == importCard.multiverseId);
                    if (card == null) card = CardData.Instance.Sets.FirstOrDefault(s => s.code.Equals(set) && s.type != "promo" && s.type != "funny" && s.type != "box").cards.FirstOrDefault(c => c.number == importCard.collectorNumber.ToString());
                    if (card.name.Equals(importCard.name))
                    {
                        deckList.Add(importCard.count + " " + card.name);
                    }
                }
                else
                {
                    var card = Helpers.CardSearch.GetCardByName(importCard.name);
                    deckList.Add(importCard.count + " " + card.name);
                }
            }
            var deckstr = string.Join("\r\n", deckList).Replace("\r\n\r\n\r\n", "\r\n\r\n");
            return deckstr;
        }

        public static ImportDeck StringToDeck(string text, ApplicationUser owner)
        {
            var deck = new ImportDeck();
            deck.Owner = owner;
            deck.MainDeck = new List<ImportCard>();
            deck.SideBoard = new List<ImportCard>();
            bool side = false;

            if (text.Contains("(") && text.Contains(")")) //treat as magic arena import
            {
                text = text.Replace("\n", "\r\n");
                foreach (var myString in text.Split(new string[] { "\r\n" }, StringSplitOptions.None))
                {
                    ImportCard card = new ImportCard();


                    if (myString.Trim().Length > 1)
                    {
                        try
                        {
                            var cardData = myString.Split(" ");
                            Regex exp = new Regex(@"(\d+)x?\s+?(.*\s?)\s+(\(.+\))?\s+(\d+)?");
                            var matches = exp.Matches(myString);

                            int.TryParse(matches[0].Groups[1].Value, out card.count);
                            card.name = matches[0].Groups[2].Value;
                            card.set = matches[0].Groups[3].Value.Replace("(DAR)", "(DOM)");
                            card.collectorNumber = matches[0].Groups[4].Value;

                            var tempCard = new Card();
                            
                            var basics = new List<string>() { "Island", "Swamp", "Mountain", "Plains", "Forest", "Остров", "Равнина", "Гора", "Лес", "Болото" };

                            if (basics.Contains(card.name))
                            {
                                switch (card.name)
                                {
                                    case "Island":
                                        {
                                            tempCard = Helpers.CardSearch.GetCardByMultiverseId(439602);
                                            break;
                                        };
                                    case "Swamp":
                                        {
                                            tempCard = Helpers.CardSearch.GetCardByMultiverseId(439603);
                                            break;
                                        };
                                    case "Mountain":
                                        {
                                            tempCard = Helpers.CardSearch.GetCardByMultiverseId(439604);
                                            break;
                                        };
                                    case "Plains":
                                        {
                                            tempCard = Helpers.CardSearch.GetCardByMultiverseId(439601);
                                            break;
                                        }
                                    case "Forest":
                                        {
                                            tempCard = Helpers.CardSearch.GetCardByMultiverseId(439605);
                                            break;
                                        }

                                }

                            }
                            else
                                {
                                tempCard = Helpers.CardSearch.GetCardByName(card.name, card.set);

                            }

                            if (tempCard != null)
                            {
                                card.multiverseId = tempCard.multiverseId;
                                card.scryfallId = tempCard.scryfallId;
                                card.tcgPlayerProductId = tempCard.tcgplayerProductId;
                            }

                            if (side) deck.SideBoard.Add(card);
                            else deck.MainDeck.Add(card);
                        }
                        catch
                        {
                            Console.WriteLine("|||||||||||||||Yep, error is somewhere here");
                        }
                    }

                    else
                    {
                        side = true;
                    }
                }
            }
            else
            {
                bool sideBoard = false;
                var deckArray = text.Replace("\r\n", "{{nl}}").Replace("\n", "\r\n").Replace("{{nl}}", "\r\n").Replace("sideboard", "\r\n\r\n", StringComparison.CurrentCultureIgnoreCase)
                    .Split(new string[] { "\r\n\r\n" },
                               StringSplitOptions.RemoveEmptyEntries);
                if (deckArray.Length > 1) sideBoard = true;

                Regex rx = new Regex(@"(\d+)x?\s+([^\(^\d]*)\s*", RegexOptions.Multiline);
                var matches = rx.Matches(deckArray[0]);
                foreach (Match match in matches)
                {
                    ImportCard importCard = new ImportCard();
                    importCard.name = match.Groups[2].Value.Trim();
                    importCard.count = int.Parse(match.Groups[1].Value.Trim());

                    var tempCard = Helpers.CardSearch.GetCardByName(importCard.name, true);
                    if (tempCard==null)
                    {
                        importCard.multiverseId = 0;
                        importCard.collectorNumber = "0";
                        importCard.set = "NONE";
                        importCard.tcgPlayerProductId = 0;
                    }
                    else
                    {
                        var set = CardData.Instance.Sets.FirstOrDefault(s => tempCard.printings.Contains(s.code) && s.type != "promo" && s.type != "box");
                        importCard.set = set.code;
                        importCard.collectorNumber = tempCard.number;
                        importCard.multiverseId = tempCard.multiverseId;
                        importCard.tcgPlayerProductId = tempCard.tcgplayerProductId;
                    }

                    deck.MainDeck.Add(importCard);
                }

                if (sideBoard)
                {
                    var matchesSB = rx.Matches(deckArray[1]);
                    foreach (Match match in matchesSB)
                    {
                        ImportCard importCard = new ImportCard();
                        importCard.name = match.Groups[2].Value.Trim();
                        importCard.count = int.Parse(match.Groups[1].Value.Trim());

                        var tempCard = Helpers.CardSearch.GetCardByName(importCard.name);
                        if (tempCard == null)
                        {
                            importCard.multiverseId = 0;
                            importCard.collectorNumber = "0";
                            importCard.set = "NONE";
                        }
                        else
                        {
                            var set = CardData.Instance.Sets.FirstOrDefault(s => tempCard.printings.Contains(s.code) && s.type != "promo" && s.type != "funny" && s.type != "box");
                            importCard.set = set.code;
                            try
                            {
                                importCard.collectorNumber = tempCard.number;
                            }
                            catch
                            {
                                importCard.collectorNumber = "0";
                            }
                            importCard.multiverseId = tempCard.multiverseId;
                            importCard.tcgPlayerProductId = tempCard.tcgplayerProductId;
                        }

                        deck.SideBoard.Add(importCard);
                    }
                }
            }

            return deck;
        }

        public static string HtmlizeDeck(ImportDeck deck)
        {
            dynamic Deck = new ExpandoObject();
            Deck.MainDeck = new Dictionary<Card, int>();
            Deck.SideBoard = new Dictionary<Card, int>();

            foreach (var card in deck.MainDeck)
            {
                Deck.MainDeck.Add(Helpers.CardSearch.GetCardByMultiverseId(card.multiverseId.GetValueOrDefault()), card.count);
            }

            if (deck.SideBoard.Count > 0)
            {
                foreach (var card in deck.SideBoard)
                {
                    Deck.SideBoard.Add(Helpers.CardSearch.GetCardByMultiverseId(card.multiverseId.GetValueOrDefault()), card.count);
                }
            }

            return HtmlizeWithDividers(Deck);
        }

        public static string HtmlizeString(string cards)
        {
            var deck = StringToDeck(cards, null);
            dynamic Deck = new ExpandoObject();
            Deck.MainDeck = new Dictionary<Card, int>();
            Deck.SideBoard = new Dictionary<Card, int>();

            foreach (var card in deck.MainDeck)
            {
                if (card.tcgPlayerProductId > 0)
                    Deck.MainDeck.Add(Helpers.CardSearch.GetCardByTcgPlayerProductId(card.tcgPlayerProductId.GetValueOrDefault()), card.count);
                else
                    Deck.MainDeck.Add(new Card() { name = card.name, multiverseId = 0, type = "Other" }, card.count);
            }

            if (deck.SideBoard.Count > 0)
            {
                foreach (var card in deck.SideBoard)
                {
                    if (card.tcgPlayerProductId > 0)
                        Deck.SideBoard.Add(Helpers.CardSearch.GetCardByTcgPlayerProductId(card.tcgPlayerProductId.GetValueOrDefault()), card.count);
                    else
                        Deck.SideBoard.Add(new Card() { name = card.name, multiverseId = 0, type = "Other" }, card.count);
                }
            }

            return HtmlizeWithDividers(Deck);
        }

        public static string HtmlizeWithDividers(dynamic deck)
        {
            if (deck != null)
            {
                Dictionary<Card, int> MainDeck = deck.MainDeck;
                Dictionary<Card, int> Creatures = MainDeck.Where(c => c.Key.type.Contains("Creature", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Creatures).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Card, int> Lands = MainDeck.Where(c => c.Key.type.Contains("Land", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Lands).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Card, int> Planeswalkers = MainDeck.Where(c => c.Key.type.Contains("Planeswalker", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Planeswalkers).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Card, int> Spells = MainDeck.Where(c => c.Key.type.Contains("Instant", StringComparison.InvariantCultureIgnoreCase) || c.Key.type.Contains("Sorcery", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Spells).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Card, int> Artifacts = MainDeck.Where(c => c.Key.type.Contains("Artifact", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Artifacts).ToDictionary(x => x.Key, x => x.Value);
                Dictionary<Card, int> Enchantments = MainDeck.Where(c => c.Key.type.Contains("Enchantment", StringComparison.InvariantCultureIgnoreCase)).ToDictionary(c => c.Key, c => c.Value);
                MainDeck = MainDeck.Except(Enchantments).ToDictionary(x => x.Key, x => x.Value);

                var all = new Dictionary<string, Dictionary<Card, int>>();
                all.Add("Creatures", Creatures);
                all.Add("Planeswalkers", Planeswalkers);
                all.Add("Spells", Spells);
                all.Add("Artifacts", Artifacts);
                all.Add("Enchantments", Enchantments);
                all.Add("Lands", Lands);
                all.Add("Other", MainDeck);


                var result = string.Empty;
                result += "<div>";
                result += "<table>";

                foreach (var item in all)
                {
                    if (item.Value.Count > 0)
                    {
                        result += "<tr><th colspan=\"3\">" + item.Key + "</th></tr>";

                        foreach (var card in item.Value)
                        {
                            if (card.Key.multiverseId > 0)
                            {
                                Card foundCard = Helpers.CardSearch.GetCardByMultiverseId(int.Parse(card.Key.multiverseId.ToString()));
                                result += "<tr><td>" + card.Value + "</td><td>" +
                                    "<a target =\"_blank\"" +
                                    "class=\"gathererTooltip\" " +
                                    "data-image=\"https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + foundCard.multiverseId + "&type=card\" " +
                                    "data-width=\"223px\"" +
                                    "data-height=\"311px\"" +
                                    "href=\"https://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=" + foundCard.multiverseId + "\"" +
                                    ">" + foundCard.name + "</a></td>";
                                result += "<td style=\"white-space:nowrap\">" + card.Key.manaCost + "</td></tr>";
                            }
                            else
                            {
                                result += "<td>" + card.Value + "</td><td colspan=\"2\">" + card.Key.name + "</td></tr>";
                            }
                        }
                    }
                }

                if (deck.SideBoard.Count > 0)
                {
                    result += "<tr><th colspan=\"3\">Sideboard</th></tr>";
                    foreach (var card in deck.SideBoard)
                    {
                        if (card.Key.multiverseId > 0)
                        {
                            Card foundCard = Helpers.CardSearch.GetCardByMultiverseId(int.Parse(card.Key.multiverseId.ToString()));
                            result += "<tr><td>" + card.Value + "</td><td>" +
                                "<a target =\"_blank\"" +
                                "class=\"gathererTooltip\" " +
                                "data-image=\"https://gatherer.wizards.com/Handlers/Image.ashx?multiverseid=" + foundCard.multiverseId + "&type=card\" " +
                                "data-width=\"223px\"" +
                                "data-height=\"311px\"" +
                                "href=\"https://gatherer.wizards.com/Pages/Card/Details.aspx?multiverseid=" + foundCard.multiverseId + "\"" +
                                ">" + foundCard.name + "</a></td>";
                            result += "<td style=\"white-space:nowrap\">" + card.Key.manaCost + "</td></tr>";
                        }
                        else
                        {
                            result += "<td>" + card.Value + "</td><td colspan=\"2\">" + card.Key.name + "</td></tr>";
                        }
                    }
                    result += "</table>";
                }
                result += "</div>";
                return ReplaceManaSymbols(result);
            }
            else
            {
                return "No cards";
            }
        }

        public static string ReplaceManaSymbols(string text)
        {
            return text.Replace("{1}", "<img class=\"mana\" src = \"/img/mana/1.svg\"/>")
                .Replace("{2}", "<img class=\"mana\" src = \"/img/mana/2.svg\"/>")
                .Replace("{3}", "<img class=\"mana\" src = \"/img/mana/3.svg\"/>")
                .Replace("{4}", "<img class=\"mana\" src = \"/img/mana/4.svg\"/>")
                .Replace("{5}", "<img class=\"mana\" src = \"/img/mana/5.svg\"/>")
                .Replace("{6}", "<img class=\"mana\" src = \"/img/mana/6.svg\"/>")
                .Replace("{7}", "<img class=\"mana\" src = \"/img/mana/7.svg\"/>")
                .Replace("{8}", "<img class=\"mana\" src = \"/img/mana/8.svg\"/>")
                .Replace("{9}", "<img class=\"mana\" src = \"/img/mana/9.svg\"/>")
                .Replace("{10}", "<img class=\"mana\" src = \"/img/mana/10.svg\"/>")
                .Replace("{11}", "<img class=\"mana\" src = \"/img/mana/11.svg\"/>")
                .Replace("{12}", "<img class=\"mana\" src = \"/img/mana/12.svg\"/>")
                .Replace("{13}", "<img class=\"mana\" src = \"/img/mana/13.svg\"/>")
                .Replace("{14}", "<img class=\"mana\" src = \"/img/mana/14.svg\"/>")
                .Replace("{15}", "<img class=\"mana\" src = \"/img/mana/15.svg\"/>")
                .Replace("{16}", "<img class=\"mana\" src = \"/img/mana/16.svg\"/>")
                .Replace("{17}", "<img class=\"mana\" src = \"/img/mana/17.svg\"/>")
                .Replace("{18}", "<img class=\"mana\" src = \"/img/mana/18.svg\"/>")
                .Replace("{19}", "<img class=\"mana\" src = \"/img/mana/19.svg\"/>")
                .Replace("{20}", "<img class=\"mana\" src = \"/img/mana/20.svg\"/>")
                .Replace("{U}", "<img class=\"mana\" src = \"/img/mana/U.svg\"/>")
                .Replace("{R}", "<img class=\"mana\" src = \"/img/mana/R.svg\"/>")
                .Replace("{G}", "<img class=\"mana\" src = \"/img/mana/G.svg\"/>")
                .Replace("{W}", "<img class=\"mana\" src = \"/img/mana/W.svg\"/>")
                .Replace("{B}", "<img class=\"mana\" src = \"/img/mana/B.svg\"/>")
                .Replace("{U/P}", "<img class=\"mana\" src = \"/img/mana/UP.svg\"/>")
                .Replace("{R/P}", "<img class=\"mana\" src = \"/img/mana/RP.svg\"/>")
                .Replace("{G/P}", "<img class=\"mana\" src = \"/img/mana/GP.svg\"/>")
                .Replace("{W/P}", "<img class=\"mana\" src = \"/img/mana/WP.svg\"/>")
                .Replace("{B/P}", "<img class=\"mana\" src = \"/img/mana/BP.svg\"/>")
                .Replace("{0}", "<img class=\"mana\" src = \"/img/mana/0.svg\"/>")
                .Replace("{X}", "<img class=\"mana\" src = \"/img/mana/X.svg\"/>")
                .Replace("{B/G}", "<img class=\"mana\" src = \"/img/mana/BG.svg\"/>")
                .Replace("{G/U}", "<img class=\"mana\" src = \"/img/mana/GU.svg\"/>")
                .Replace("{G/W}", "<img class=\"mana\" src = \"/img/mana/GW.svg\"/>")
                .Replace("{R/G}", "<img class=\"mana\" src = \"/img/mana/RG.svg\"/>")
                .Replace("{R/W}", "<img class=\"mana\" src = \"/img/mana/RW.svg\"/>")
                .Replace("{U/R}", "<img class=\"mana\" src = \"/img/mana/UR.svg\"/>")
                .Replace("{U/B}", "<img class=\"mana\" src = \"/img/mana/UB.svg\"/>")
                .Replace("{W/U}", "<img class=\"mana\" src = \"/img/mana/WU.svg\"/>")
                .Replace("{W/B}", "<img class=\"mana\" src = \"/img/mana/WB.svg\"/>")
                .Replace("{B/R}", "<img class=\"mana\" src = \"/img/mana/BR.svg\"/>")
                .Replace("{C}", "<img class=\"mana\" src = \"/img/mana/C.svg\"/>")
                .Replace("{S}", "<img class=\"mana\" src = \"/img/mana/S.svg\"/>")
                .Replace("{2/U}", "<img class=\"mana\" src = \"/img/mana/2U.svg\"/>")
                .Replace("{2/R}", "<img class=\"mana\" src = \"/img/mana/2R.svg\"/>")
                .Replace("{2/B}", "<img class=\"mana\" src = \"/img/mana/2B.svg\"/>")
                .Replace("{2/G}", "<img class=\"mana\" src = \"/img/mana/2G.svg\"/>")
                .Replace("{2/W}", "<img class=\"mana\" src = \"/img/mana/2W.svg\"/>")
                ;


        }
    }

    public class ImportCard
    {
        public string name;
        public int count;
        public string set;
        public string collectorNumber;
        public int? multiverseId;
        public string scryfallId;
        public int? tcgPlayerProductId;

        public override bool Equals(object obj)
        {
            var item = obj as ImportCard;

            if (item == null)
            {
                return false;
            }

            return this.name.Equals(item.name) && this.set.Equals(item.set) && this.collectorNumber == item.collectorNumber;
        }

        public override int GetHashCode()
        {
            return (this.name + this.set).GetHashCode();
        }
    }
}