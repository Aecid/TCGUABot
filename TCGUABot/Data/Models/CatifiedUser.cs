using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data
{
    public class CatifiedUser
    {
        [Key]
        public long? TelegramId { get; set; }
    }
}
