using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data
{
    public class Product
    {
        [Key]
        public int ProductId { get; set; }
        public string Name { get; set; }
        public string CleanName { get; set; }
        public string ImageUrl { get; set; }
        public int GroupId { get; set; }
        public string Url { get; set; }
        public string ExtendedData { get; set; }
    }
}
