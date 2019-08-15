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
                if (!string.IsNullOrEmpty(set))
                {
                    var card = CardData.Instance.Sets.FirstOrDefault(s => s.code.Equals(set)).cards.FirstOrDefault(c => c.number == importCard.collectorNumber.ToString());
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
                    var card = CardData.Instance.Sets.FirstOrDefault(s => s.code.Equals(set)).cards.FirstOrDefault(c => c.number == importCard.collectorNumber.ToString());
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
                foreach (var myString in text.Split(new string[] { "\r\n" }, StringSplitOptions.None))
                {
                    ImportCard card = new ImportCard();

                    if (myString.Trim().Length > 1)
                    {
                        var cardData = myString.Split(" ");
                        Regex exp = new Regex(@"(\d+)x?\s+?(.*\s?)\s+(\(.+\))?\s+(\d+)?");
                        var matches = exp.Matches(myString);

                        int.TryParse(matches[0].Groups[1].Value, out card.count);
                        card.name = matches[0].Groups[2].Value;
                        card.set = matches[0].Groups[3].Value;
                        int.TryParse(matches[0].Groups[4].Value, out card.collectorNumber);

                        var tempCard = Helpers.CardSearch.GetCardByName(card.name, card.set);
                        if (tempCard != null) card.multiverseId = tempCard.multiverseId;

                        if (side) deck.SideBoard.Add(card);
                        else deck.MainDeck.Add(card);
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
                var deckArray = text.Replace("sideboard", "\r\n\r\n", StringComparison.CurrentCultureIgnoreCase)
                    .Split(new string[] { "\r\n\r\n" },
                               StringSplitOptions.RemoveEmptyEntries);
                if (deckArray.Length > 1) sideBoard = true;

                Regex rx = new Regex(@"(\d+)x?\s+([^\(^\d]*)\s*", RegexOptions.Multiline);
                var matches = rx.Matches(deckArray[0]);
                foreach (Match match in matches)
                {
                    ImportCard importCard = new ImportCard();
                    importCard.name = match.Groups[2].Value.Trim();

                    var tempCard = Helpers.CardSearch.GetCardByName(importCard.name);
                    var set = CardData.Instance.Sets.FirstOrDefault(s => tempCard.printings.Contains(s.code) && s.type != "promo" && s.type != "funny");
                    importCard.set = set.code;
                    importCard.collectorNumber = int.Parse(tempCard.number);
                    importCard.count = int.Parse(match.Groups[1].Value.Trim());
                    importCard.multiverseId = tempCard.multiverseId;

                    deck.MainDeck.Add(importCard);
                }

                if (sideBoard)
                {
                    var matchesSB = rx.Matches(deckArray[1]);
                    foreach (Match match in matchesSB)
                    {
                        ImportCard importCard = new ImportCard();
                        importCard.name = match.Groups[2].Value.Trim();

                        var tempCard = Helpers.CardSearch.GetCardByName(importCard.name);
                        var set = CardData.Instance.Sets.FirstOrDefault(s => tempCard.printings.Contains(s.code) && s.type != "promo" && s.type != "funny");
                        importCard.set = set.code;
                        importCard.collectorNumber = int.Parse(tempCard.number);
                        importCard.count = int.Parse(match.Groups[1].Value.Trim());
                        importCard.multiverseId = tempCard.multiverseId;


                        deck.SideBoard.Add(importCard);
                    }
                }
            }

            return deck;
        }
    }

    public class ImportCard
    {
        public string name;
        public int count;
        public string set;
        public int collectorNumber;
        public int? multiverseId;

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