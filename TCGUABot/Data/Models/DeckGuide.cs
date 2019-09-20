using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data.Models
{
    public class DeckGuide
    {
        [Key]
        public int Id { get; set; }
        public string[] Keywords { get; set; }
        public string Format { get; set; }
        public string Url { get; set; }
        public DateTime LastUpdated { get; set; }

        [NotMapped]
        public string KeywordsSeparated { get; set; }
    }
}