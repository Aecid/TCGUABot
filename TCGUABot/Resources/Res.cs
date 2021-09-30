using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Resources
{
    public abstract class Res
    {
        public abstract string cardNotFoundByRequest { get; }
        public abstract string tryAtTcgua { get; }
        public abstract string price { get; }
        public abstract string priceFoil { get; }
        public abstract string marketPrice { get; }
        public abstract string marketPriceFoil { get; }
        public abstract string priceNoData { get; }
        public abstract string entryFee { get; }
        public abstract string rewards { get; }
        public abstract string free { get; }
        public abstract string ruFlag { get; }
        public abstract string enFlag { get; }
        public abstract string rulingsNotFound { get; }
        public abstract string importError { get; }
        public abstract string deckLink { get; }
        public abstract string maxPlayers { get; }
        public abstract string legality { get; }

    }
}
