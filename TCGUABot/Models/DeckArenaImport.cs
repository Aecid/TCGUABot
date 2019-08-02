using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models
{
    public class DeckArenaImport
    {
        public List<ArenaCard> MainDeck;
        public List<ArenaCard> SideBoard;
    }

    public class ArenaCard
    {
        public string name;
        public int count;
        public string set;
        public int collectorNumber;

        public override bool Equals(object obj)
        {
            var item = obj as ArenaCard;

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