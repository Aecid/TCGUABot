﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Resources
{
    public class ResUa : Res
    {
        public override string cardNotFoundByRequest => "Карта не знайдена за запитом";
        public override string tryAtTcgua => "Спробуйте ввести в чат \"@tcgua_bot ім'якарти\" і почекати підказку від бота";
        public override string price => "Медіанна ціна";
        public override string priceFoil => "Медіанна ціна фойли";
        public override string priceNoData => "Немає даних про ціну";
        public override string entryFee => "Вартість";
        public override string rewards => "Призи";
        public override string ruFlag => "🏳‍🌈";
        public override string enFlag => "🇺🇸";
        public override string rulingsNotFound => "Рулінги не знайдені";
        public override string importError => "Помилка імпорту деки";
        public override string deckLink => "Посилання на деку";
        public override string free => "Безкоштовно!";
        public override string maxPlayers => "Максимум гравців";
        public override string legality => "Легальность";

        public override string marketPrice => "Ринкова ціна";

        public override string marketPriceFoil => "Ринкова ціна фойли";
    }
}
