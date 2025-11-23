using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Server.Models;

namespace Server.Database {
    /// <summary>
    /// Represents the Entity Framework Core database context for the Wordle application.
    /// </summary>
    /// <remarks>
    /// This context extends <see cref="IdentityDbContext{TUser}"/> to include identity management
    /// using <see cref="WordleUser"/> as the user entity.
    /// <para>
    /// It defines all persisted entities such as games, attempts, achievements, leaderboard entries,
    /// and word metadata. Relationship configurations and constraints are applied in
    /// <see cref="OnModelCreating(ModelBuilder)"/>.
    /// </para>
    /// </remarks>
    public class WordleDbContext : IdentityDbContext<WordleUser> {
        /// <summary>
        /// Achievements available in the game.
        /// </summary>
        public DbSet<Achievement> Achievements { get; set; }

        /// <summary>
        /// Daily challenge definitions associated with specific words.
        /// </summary>
        public DbSet<DailyChallenge> DailyChallenges { get; set; }

        /// <summary>
        /// All game sessions played by users.
        /// </summary>
        public DbSet<Game> Games { get; set; }

        /// <summary>
        /// Individual attempts submitted during a game.
        /// </summary>
        public DbSet<GameAttempt> GameAttempts { get; set; }

        /// <summary>
        /// Leaderboard entries used for ranking players.
        /// </summary>
        public DbSet<Leaderboard> Leaderboards { get; set; }

        /// <summary>
        /// Many-to-many join linking users to achievements they have unlocked.
        /// </summary>
        public DbSet<UserAchievements> UserAchievements { get; set; }

        /// <summary>
        /// All valid words in the game dictionary.
        /// </summary>
        public DbSet<Word> Words { get; set; }

        /// <summary>
        /// Word categories used to classify and filter words.
        /// </summary>
        public DbSet<WordCategory> WordCategories { get; set; }

        /// <summary>
        /// Stores the settings and preferences for each user.
        /// </summary>
        public DbSet<WordleSettings> WordleSettings { get; set; }

        /// <summary>
        /// Stores validated dictionary words to ensure uniqueness and integrity.
        /// </summary>
        public DbSet<WordsValidation> WordsValidations { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WordleDbContext"/> class.
        /// </summary>
        /// <param name="options">Configuration options for the DbContext.</param>
        public WordleDbContext(DbContextOptions<WordleDbContext> options) : base(options) {

        }


        /// <summary>
        /// Configures model relationships, constraints, and indexes for the Wordle database schema.
        /// </summary>
        /// <param name="modelBuilder">The builder used to configure entity types.</param>
        /// <remarks>
        /// This method applies:
        /// <list type="bullet">
        /// <item><description>One-to-many relations between <see cref="Word"/> and <see cref="WordCategory"/>.</description></item>
        /// <item><description>One-to-many relations between <see cref="Game"/> and <see cref="WordleUser"/>.</description></item>
        /// <item><description>One-to-many relations between <see cref="Game"/> and <see cref="Word"/>.</description></item>
        /// <item><description>One-to-one relations for daily challenges and user settings.</description></item>
        /// <item><description>Many-to-many relations via <see cref="UserAchievements"/>.</description></item>
        /// <item><description>Cascade delete and restrictive delete rules where appropriate.</description></item>
        /// <item><description>A uniqueness constraint on validated dictionary words.</description></item>
        /// </list>
        /// </remarks>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
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

            modelBuilder.Entity<WordsValidation>()
                .HasIndex(wv => wv.Text)
                .IsUnique();
        }

    }
}
