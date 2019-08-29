using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data.Models
{
    public class Tournament
    {
        [Key]
        public string Id { get; set; }

        public DateTime CreationDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime PlannedDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsLocked { get; set; }
        public bool IsClosed { get; set; }
        public string CreatorId { get; set; }
        public ICollection<TournamentUserPair> TournamentUserPairs { get; set; }
    }
}
