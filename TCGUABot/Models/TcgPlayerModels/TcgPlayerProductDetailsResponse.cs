using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models.TcgPlayerModels
{
    public class TcgPlayerProductDetailsResponse
    {
        public bool success { get; set; }
        public List<TcgPlayerProductDetails> results { get; set; }
    }
}
