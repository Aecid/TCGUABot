using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data
{
    public class TournamentUserPair
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public long? PlayerTelegramId { get; set; }
        public string PlayerId { get; set; }
        public string DeckId { get; set; }

        [ForeignKey("TournamentId")]
        public string TournamentId { get; set; }
    }
}
