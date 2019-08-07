using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Models
{
    public class AppUser : IdentityUser
    {
        [Required]
        [StringLength(156)]
        public long TelegramId;
    }
}
