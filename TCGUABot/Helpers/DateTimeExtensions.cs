using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Helpers
{
    public static class DateTimeExtensions
    {
        public static DateTime Next(this DateTime from, DayOfWeek dayOfWeek)
        {
            int start = (int)from.DayOfWeek;
            int target = (int)dayOfWeek;
            if (target <= start)
                target += 7;
            return from.AddDays(target - start);
        }
    }
}
