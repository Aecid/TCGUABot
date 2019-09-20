using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Resources
{
    public static class Lang
    {
        public static Res Res(string lang="ru")
        {
            switch (lang)
            {
                case "ru":
                    return new ResRu();
                case "en":
                    return new ResEn();
                case "ua":
                    return new ResUa();
                default:
                    return new ResRu();
            }
        }
    }
}
