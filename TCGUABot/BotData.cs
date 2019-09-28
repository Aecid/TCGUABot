using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot
{
    public static class BotData
    {
        public static List<string> ComprehensiveRules { get; set; }

        static BotData()
        {
            var logFile = File.ReadAllLines("Files/CompRules.txt");
            ComprehensiveRules = new List<string>(logFile).Where(s => !string.IsNullOrWhiteSpace(s)).Distinct().ToList();
        }
    }
}
