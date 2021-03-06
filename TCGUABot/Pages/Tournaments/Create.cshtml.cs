﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TCGUABot.Data;
using TCGUABot.Data.Models;

namespace TCGUABot.Pages.Tournaments
{
    [Authorize(Roles = "Admin,Store Owner,Judge,Event Organizer,Igor Modern Admin")]
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public CreateModel(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Tournament Tournament { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Tournament.CreationDate = TimeService.GetLocalTime();
            var user = await _userManager.GetUserAsync(User);
            Tournament.CreatorId = user.Id;
            Tournament.LocationId = 1;

            var telegramId = _context.UserLogins.FirstOrDefault(u => u.UserId == user.Id)?.ProviderKey;
            if (telegramId == "305751207") Tournament.LocationId = 1;
            if (telegramId == "73379396") Tournament.LocationId = 2;
            _context.Tournaments.Add(Tournament);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}