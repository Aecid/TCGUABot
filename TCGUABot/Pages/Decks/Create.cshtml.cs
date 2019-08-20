using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using TCGUABot.Data;
using TCGUABot.Data.Models;

namespace TCGUABot.Pages.Decks
{
    public class CreateModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        public CreateModel(TCGUABot.Data.ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Deck Deck { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (this.Deck.Cards == string.Empty)
            {
                return new UnprocessableEntityResult();
            }

            //            < input type = "hidden" asp -for= "Deck.ApplicationUser" value = "@user" class="form-control" />
            //<input type = "hidden" asp-for="Deck.UserId" value="@user.Id" class="form-control" />
            ApplicationUser user;
            if (User.Identity.IsAuthenticated)
            {
                user = await _userManager.GetUserAsync(User);
            }
            else
            {
                user = _context.Users.FirstOrDefault(u => u.Id == "d34f08f5-9daa-46d6-a87c-cc3a6fda538a");
            }
            this.Deck.ApplicationUser = user;
            this.Deck.UserId = user.Id;
            this.Deck.CreationDate = DateTime.UtcNow;
            _context.Decks.Add(Deck);
            var z = await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}