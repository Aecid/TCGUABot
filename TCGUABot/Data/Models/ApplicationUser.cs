﻿using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TCGUABot.Data.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        public string DCI { get; set; }
    }
}
