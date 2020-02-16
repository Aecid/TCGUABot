using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data
{
    public class Order
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public long? PlayerTelegramId { get; set; }
        public int? ProductId { get; set; }
        public string ProductName { get; set; }
        public string Edition { get; set; }
        public bool isFoil { get; set; }
        public string Lang { get; set; }
        public bool Wts { get; set; }
        public bool isOpen { get; set; }

        public DateTime Created { get; set; }
        public DateTime LastModified { get; set; }



    }
}
