using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TCGUABot.Controllers;
using TCGUABot.Data.Models;

namespace TCGUABot.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            var deckListController = new DecklistController(this);
        }

        public DbSet<Deck> Decks { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentUserPair> TournamentUserPairs { get; set; }
        public DbSet<TelegramUser> TelegramUsers { get; set; }
        public DbSet<TelegramChat> TelegramChats { get; set; }
        public DbSet<MythicSpoiler> MythicSpoilers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ForNpgsqlUseIdentityColumns();
            builder.Entity<TournamentUserPair>()
                .Property(p => p.Id)
                .ValueGeneratedOnAdd();

            builder.HasPostgresExtension("uuid-ossp")
                   .Entity<Tournament>()
                   .Property(e => e.Id)
                   .HasDefaultValueSql("uuid_generate_v4()");

            base.OnModelCreating(builder);
        }
    }
}
