using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Models;

namespace TCGUABot.Helpers
{
    public static class CardSearch
    {
        public static Card GetCardByName(string name, bool exactMatch = false)
        {
            Card card = new Card();
            if (!exactMatch)
            {
                var set = CardData.Instance.Sets.FirstOrDefault(s => s.cards.Any(c => c.name.ToLowerInvariant().Contains(name.ToLowerInvariant()) && c.multiverseId > 0));
                if (set == null)
                {
                    set = CardData.Instance.Sets.FirstOrDefault(s => s.cards.Any(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Contains(name.ToLowerInvariant()) && f.multiverseId > 0)));
                }
                if (set != null)
                {
                    card = set.cards.FirstOrDefault(c => c.name.ToLowerInvariant().Contains(name.ToLowerInvariant()) && c.multiverseId > 0);
                    if (card == null)
                    {
                        card = set.cards.FirstOrDefault(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Contains(name.ToLowerInvariant()) && f.multiverseId > 0));
                    }

                    return card;
                }
                else
                {
                    return null;
                }
            }
            else
            {
                var set = CardData.Instance.Sets.FirstOrDefault(s => s.cards.Any(c => c.name.ToLowerInvariant().Equals(name.ToLowerInvariant()) && c.multiverseId > 0));
                if (set == null)
                {
                    set = CardData.Instance.Sets.FirstOrDefault(s => s.cards.Any(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Equals(name.ToLowerInvariant()) && f.multiverseId > 0)));
                }
                if (set != null)
                {
                    card = set.cards.FirstOrDefault(c => c.name.ToLowerInvariant().Equals(name.ToLowerInvariant()) && c.multiverseId > 0);
                    if (card == null)
                    {
                        card = set.cards.FirstOrDefault(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Equals(name.ToLowerInvariant()) && f.multiverseId > 0));
                    }

                    return card;
                }
                else
                {
                    return null;
                }
            }
        }

        public static Card GetCardByName(string name, string set)
        {
            Card card = new Card();
            set = set.Replace("(", "").Replace(")", "").Replace("DAR", "DOM");

            var setToSearch = CardData.Instance.Sets.FirstOrDefault(s => s.code.ToLower().Equals(set.ToLower()));
            if (setToSearch == null)
            {
                return null;
            }
            if (setToSearch != null)
            {

                card = setToSearch.cards.FirstOrDefault(c => c.name.ToLowerInvariant().Equals(name.ToLowerInvariant()));
                if (card == null)
                {
                    card = setToSearch.cards.FirstOrDefault(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Equals(name.ToLowerInvariant())));
                }


                return card;
            }
            else
            {
                return null;
            }
        }

        public static Card GetCardByMultiverseId(int id)
        {
            foreach (var set in CardData.Instance.Sets)
            {
                if (set.cards.Any(c => c.multiverseId == id)) return set.cards.FirstOrDefault(c => c.multiverseId == id);
                if (set.cards.Any(c => c.foreignData.Any(f => f.multiverseId == id))) return set.cards.FirstOrDefault(c => c.foreignData.Any(f => f.multiverseId == id));
            }
            return null;
        }
    }
}
