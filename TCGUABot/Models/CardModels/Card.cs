using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models
{
    public class Card
    {
        //public string artist;
        //public string borderColor;
        //public List<string> colorIdentity;
        //public List<string> colors;
        public double convertedManaCost;
        public List<ForeignData> foreignData;
        //public string frameVersion;
        //public bool hasFoil;
        //public bool hasNoFoil;
        //public bool isMtgo;
        public bool isPromo;
        //public bool isPaper;
        //public bool isReprint;
        //public string layout;
        //public Dictionary<string, string> legalities;
        public string manaCost;
        //public int mcmId;
        //public int mcmMetaId;
        //public int mtgoFoilId;
        //public int mtgoId;
        //public int mtgstocksId;
        public int multiverseId;
        public string name;
        public string number;
        //public string originalText;
        //public string originalType;
        //public Prices prices;
        //public string rarity;
        public List<string> printings;
        //public Dictionary<string, string> purchaseUrls;
        public List<Ruling> rulings;
        public string scryfallId;
        public string scryfallIllustrationId;
        public string scryfallOrcaleId;
        public List<string> subtypes;
        public List<string> supertypes;
        public int tcgplayerProductId;
        //public string tcgplayerPurchaseUrl;
        public string text;
        public string type;
        //public List<string> types;
        //public string uuid;
        //public string power;
        //public string toughness;

        public string ruName
        {
            get
            {
                if (this.foreignData.Any(f => f.language == "Russian"))
                    return this.foreignData.FirstOrDefault(f => f.language.Equals("Russian")).name;
                else return this.name;
            }
        }

        public int ruMultiverseId
        {
            get
            {
                if (this.foreignData.Any(f => f.language == "Russian"))
                    return this.foreignData.FirstOrDefault(f => f.language.Equals("Russian")).multiverseId;
                else return this.multiverseId;
            }
        }

        public override bool Equals(object obj)
        {
            var item = obj as Card;

            if (item == null)
            {
                return false;
            }

            return this.name.Equals(item.name);
        }

        public override int GetHashCode()
        {
            return this.multiverseId.GetHashCode();
        }
    }

    public class ForeignData
    {
        public string language;
        public int multiverseId;
        public string name;
        //public string text;
        //public string type;
        //public string flavorText;
    }

    public class Ruling
    {
        public string date;
        public string text;
    }

    public class Prices
    {
        public Dictionary<string, float> paper;
        public Dictionary<string, float> paperFoil;
    }
}