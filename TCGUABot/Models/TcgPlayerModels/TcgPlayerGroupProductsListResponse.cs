using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models.TcgPlayerModels
{
    public class TcgPlayerGroupProductsListResponse
    {
        public bool success { get; set; }
        public int totalItems { get; set; }
        public List<TcgPlayerProductDetails> results { get; set; }
    }
}
