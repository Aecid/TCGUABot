using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models.TcgPlayerModels
{
    public class TcgPlayerGroup
    {
        public int groupId { get; set; }
        public string name { get; set; }
        public string abbreviation { get; set; }
        public bool isSupplemental { get; set; }
    }
}
