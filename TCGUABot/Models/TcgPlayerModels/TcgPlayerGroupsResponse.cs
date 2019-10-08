using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models.TcgPlayerModels
{
    public class TcgPlayerGroupsResponse
    {
        public int totalItems { get; set; }
        public bool success { get; set; }
        public List<TcgPlayerGroup> results;
    }
}
