using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data.Models
{
    public class MythicSpoiler
    {
        [Key]
        public string Url { get; set; }
    }
}
