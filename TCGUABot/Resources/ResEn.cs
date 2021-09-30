using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Resources
{
    public class ResEn : Res
    {
        public override string cardNotFoundByRequest => "Card not found by request";
        public override string tryAtTcgua => "Try to enter \"@tcgua_bot cardname\" into chat and wait for bot's suggestions";
        public override string price => "Median price";
        public override string priceFoil => "Median foil price";
        public override string priceNoData => "No price data";
        public override string entryFee => "Entry fee";
        public override string rewards => "Rewards";
        public override string ruFlag => "🇷🇺";
        public override string enFlag => "🇺🇸";
        public override string rulingsNotFound => "Rulings not found";
        public override string importError => "Deck import error";
        public override string deckLink => "Deck link";
        public override string free => "Free!";
        public override string maxPlayers => "Max players";
        public override string legality => "Legality";

        public override string marketPrice => "Market price";

        public override string marketPriceFoil => "Market foil price";
    }
}
