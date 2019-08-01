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
    }
}