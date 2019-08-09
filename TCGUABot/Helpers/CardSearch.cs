using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TCGUABot.Models;

namespace TCGUABot.Helpers
{
    public static class CardSearch
    {
        public static Card GetCardByName(string name)
        {
            Card card = new Card();

            var set = CardData.Instance.Sets.FirstOrDefault(s => s.cards.Any(c => c.name.ToLowerInvariant().Contains(name.ToLowerInvariant())));
            if (set == null)
            {
                set = CardData.Instance.Sets.FirstOrDefault(s => s.cards.Any(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Contains(name.ToLowerInvariant()))));
            }
            if (set != null)
            {
                card = set.cards.FirstOrDefault(c => c.name.ToLowerInvariant().Contains(name.ToLowerInvariant()));
                if (card == null)
                {
                    card = set.cards.FirstOrDefault(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Contains(name.ToLowerInvariant())));
                }

                return card;
            }
            else
            {
                return null;
            }
        }

        public static Card GetCardByName(string name, string set)
        {
            Card card = new Card();

            var setToSearch = CardData.Instance.Sets.FirstOrDefault(s => s.code.ToLower().Equals(set.ToLower()));
            if (setToSearch == null)
            {
                return null;
            }
            if (setToSearch != null)
            {
                card = setToSearch.cards.FirstOrDefault(c => c.name.ToLowerInvariant().Contains(name.ToLowerInvariant()));
                if (card == null)
                {
                    card = setToSearch.cards.FirstOrDefault(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Contains(name.ToLowerInvariant())));
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
