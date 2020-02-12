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
            var expansions = new List<string>()
                    {
                        "core", "draft_innovation", "expansion"
                    };

            if (string.IsNullOrEmpty(name)) return null;

            Card card = new Card();
            if (!exactMatch)
            {
                var set = CardData.Instance.Sets.LastOrDefault(
                s => s.cards.Any
                (

                        c =>
                        (
                            (
                                c.name.ToLowerInvariant().Equals(name.ToLowerInvariant())
                            )
                        ||
                            (
                                c.foreignData.Any(f => f.name.ToLowerInvariant().Equals(name.ToLowerInvariant()))
                            )
                        )
                        &&
                        c.tcgplayerProductId > 0

                )
                &&
                expansions.Contains(s.type)
                );

                if (set != null)
                {
                    card = set.cards.LastOrDefault(c => c.name.ToLowerInvariant().Equals(name.ToLowerInvariant()));
                    if (card == null)
                    {
                        card = set.cards.LastOrDefault(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Equals(name.ToLowerInvariant())));
                    }

                    card.Set = set.code;

                    return card;
                }
                else
                {

                    set = CardData.Instance.Sets.LastOrDefault(s => s.cards.Any(c => c.name.ToLowerInvariant().Contains(name.ToLowerInvariant())) && expansions.Contains(s.type));
                    if (set == null)
                    {
                        set = CardData.Instance.Sets.LastOrDefault(s => s.cards.Any(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Contains(name.ToLowerInvariant()))));
                    }
                    if (set != null)
                    {
                        card = set.cards.LastOrDefault(c => c.name.ToLowerInvariant().Contains(name.ToLowerInvariant()));
                        if (card == null)
                        {
                            card = set.cards.LastOrDefault(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Contains(name.ToLowerInvariant())));
                        }

                        card.Set = set.code;
                        return card;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            else
            {
                var set = CardData.Instance.Sets.LastOrDefault(
                    s => s.cards.Any
                    (

                            c =>
                            (
                            c.name.ToLowerInvariant().Equals(name.ToLowerInvariant())
                            )
                            ||
                            (
                            c.foreignData.Any(f => f.name.ToLowerInvariant().Equals(name.ToLowerInvariant()))
                            )

                    )
                    &&
                    expansions.Contains(s.type)
                    );

                if (set != null)
                {
                    card = set.cards.LastOrDefault(c => c.name.ToLowerInvariant().Equals(name.ToLowerInvariant()));
                    if (card == null)
                    {
                        card = set.cards.LastOrDefault(c => c.foreignData.Any(f => f.name.ToLowerInvariant().Equals(name.ToLowerInvariant())));
                    }

                    card.Set = set.code;

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

                if (card == null)
                {
                    List<string> setTypes = new List<string>() { "archenemy", "commander", "duel_deck", "from_the_vault", "funny", "masters", "masterpiece", "memorabilia", "spellbook", "planechase", "premium_deck", "promo", "token", "treasure_chest", "vanguard" };
                    var nonFunnySets = CardData.Instance.Sets.Where(s => !setTypes.Contains(s.type));
                    var nonFunnySetToSerach = nonFunnySets.FirstOrDefault(s => s.cards.Any(z => z.name.Equals(name, StringComparison.InvariantCultureIgnoreCase)));
                    card = nonFunnySetToSerach.cards.FirstOrDefault(z => z.name.Equals(name, StringComparison.InvariantCultureIgnoreCase));
                }

                if (card == null) return null;

                card.Set = set;

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

        public static Card GetCardByTcgPlayerProductId(int id)
        {
            foreach (var set in CardData.Instance.Sets)
            {
                var card = set.cards.FirstOrDefault(c => c.tcgplayerProductId == id);
                if (card != null)
                {
                    card.Set = set.code;
                    return card;
                }
            }

            return null;
        }
    }
}
