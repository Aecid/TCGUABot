using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data.Models
{
    public class TradingCard
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int ProductId { get; set; }

        public string Name { get; set; }
        public string Set { get; set; }
        public bool IsFoil { get; set; }
        public string Notes { get; set; }
        public float Price { get; set; }
        public string State { get; set; }

        public long OwnerTelegramId { get; set; }
    }
}
