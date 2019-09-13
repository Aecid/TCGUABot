using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data.Models
{
    public class TelegramChat
    {
        [Key]
        public long Id { get; set; }

        public string Language { get; set; }
        public bool SendSpoilers { get; set; }
    }
}
