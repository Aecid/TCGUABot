using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string DCI { get; set; }

        [NotMapped]
        public string TelegramId { get; set; }
    }
}
