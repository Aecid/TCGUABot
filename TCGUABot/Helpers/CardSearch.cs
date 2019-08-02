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

            var set = CardData.Instance.Sets.FirstOrDefault(s => s.Value.cards.Any(c => c.name.ToLowerInvariant().Contains(name.ToLowerInvariant()))).Value;
            if (set == null)
            {
                set = CardData.Instance.Sets.FirstOrDefault(s => s.Value.cards.Any(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Contains(name.ToLowerInvariant())))).Value;
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
    }
}
