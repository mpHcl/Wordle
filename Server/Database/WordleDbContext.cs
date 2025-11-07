using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Database
{
    public class WordleDbContext : IdentityDbContext<WordleUser>
    {
        public DbSet<Achievements> Achievements { get; set; }
        public DbSet<DailyChallenge> DailyChallenges { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<GameAttempt> GameAttempts { get; set; }
        public DbSet<Leaderboard> Leaderboards { get; set; }
        public DbSet<UserAchievements> UserAchievements { get; set; }
        public DbSet<Word> Words { get; set; }
        public DbSet<WordCategory> WordCategories { get; set; }
        public DbSet<WordleSettings> WordleSettings { get; set; }

        public WordleDbContext(DbContextOptions<WordleDbContext> options) : base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // --- Word ↔ Category
            modelBuilder.Entity<Word>()
                .HasOne(w => w.Category)
                .WithMany(c => c.Words) // or you can add ICollection<Word> in WordCategory if needed
                .HasForeignKey(w => w.CategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Game ↔ User (many games per user)
            modelBuilder.Entity<Game>()
                .HasOne(g => g.User)
                .WithMany(u => u.Games) // or ICollection<Game> in WordleUser
                .HasForeignKey(g => g.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Game ↔ Word (many games per word)
            modelBuilder.Entity<Game>()
                .HasOne(g => g.Word)
                .WithMany()
                .HasForeignKey(g => g.WordId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Game>()
                .HasOne(g => g.DailyChallenge)
                .WithMany()
                .HasForeignKey(g => g.DailyChallangeId)
                .OnDelete(DeleteBehavior.Restrict);

            // --- GameAttempt ↔ Game (many attempts per game)
            modelBuilder.Entity<GameAttempt>()
                .HasOne(ga => ga.Game)
                .WithMany(g => g.Attempts) // or ICollection<GameAttempt> in Game
                .HasForeignKey(ga => ga.GameId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- Leaderboard ↔ User (one per user)
            modelBuilder.Entity<Leaderboard>()
                .HasOne(l => l.User)
                .WithOne()
                .HasForeignKey<Leaderboard>(l => l.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- WordleUser ↔ WordleSettings (1:1)
            modelBuilder.Entity<WordleUser>()
                .HasOne(u => u.Settings)
                .WithOne()
                .HasForeignKey<WordleUser>(u => u.SettingsId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- UserAchievements (many-to-many via join entity)
            modelBuilder.Entity<UserAchievements>()
                .HasKey(ua => new { ua.UserId, ua.AchievementId });

            modelBuilder.Entity<UserAchievements>()
                .HasOne(ua => ua.User)
                .WithMany(u => u.UserAchievements)
                .HasForeignKey(ua => ua.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<UserAchievements>()
                .HasOne(ua => ua.Achievement)
                .WithMany()
                .HasForeignKey(ua => ua.AchievementId)
                .OnDelete(DeleteBehavior.Cascade);

            // --- DailyChallenge ↔ Word (1:1)
            modelBuilder.Entity<DailyChallenge>()
                .HasOne(dc => dc.Word)
                .WithMany()
                .HasForeignKey(dc => dc.WordId)
                .OnDelete(DeleteBehavior.Restrict);
        }

    }
}
