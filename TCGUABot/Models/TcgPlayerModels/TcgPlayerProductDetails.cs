using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models.TcgPlayerModels
{
    public class TcgPlayerProductDetails
    {
        public int productId { get; set; }
        public string name { get; set; }
        public string cleanName { get; set; }
        public string imageUrl { get; set; }
        public int groupId { get; set; }
        public string url { get; set; }
        public dynamic extendedData { get; set; }
    }
}
