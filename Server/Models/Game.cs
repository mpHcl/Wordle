using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models {
    /// <summary>
    /// Represents a single game session. Contains information about the player,
    /// the assigned word, game settings, attempts, and daily challenge association.
    /// </summary>
    /// <remarks>
    /// This model is persisted to the database.
    /// The <see cref="UserId"/> and <see cref="WordId"/> properties are required 
    /// and must be provided when creating a new game.
    /// The navigation properties <see cref="User"/>, <see cref="Word"/> and 
    /// <see cref="DailyChallenge"/> are automatically populated by Entity Framework
    /// when included explicitly via the <c>Include</c> method.
    /// </remarks>
    public class Game {
        /// <summary>
        /// Primary key for the game.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key referencing the owner of the game.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Navigation property representing the player who owns the game.
        /// Loaded automatically by EF when included in queries.
        /// </summary>
        public WordleUser? User { get; set; }

        /// <summary>
        /// Foreign key referencing the word assigned to this game.
        /// </summary>
        public required int WordId { get; set; }

        /// <summary>
        /// Navigation property for the word used in the game.
        /// Available when explicitly included via <c>.Include(g => g.Word)</c>.
        /// </summary>
        public Word? Word { get; set; }

        /// <summary>
        /// Optional foreign key referencing a daily challenge this game belongs to.
        /// Only set if game is a daily challenge game, otherwise null.
        /// </summary>
        public int? DailyChallangeId { get; set; }

        /// <summary>
        /// Navigation property for the daily challenge associated with this game,
        /// if applicable.
        /// </summary>
        public DailyChallenge? DailyChallenge { get; set; }

        /// <summary>
        /// Indicates whether the game was played in hard mode.
        /// Should be set based on players settings. 
        /// </summary>
        public required bool HardMode { get; set; }

        /// <summary>
        /// Indicates whether the player enabled hints during the game.
        /// Should be set based on players settings.
        /// </summary>
        public required bool Hints { get; set; }

        /// <summary>
        /// Indicates whether the game was successfully completed (word guessed).
        /// </summary>
        public bool IsWon { get; set; } = false;


        /// <summary>
        /// Collection of attempts the player made during the game.
        /// </summary>
        public ICollection<GameAttempt> Attempts { get; set; } = [];


        /// <summary>
        /// Gets the number of attempts made in the game.
        /// This value is not stored in the database.
        /// </summary>
        [NotMapped]
        public int NumebrOfAttempts => Attempts.Count;

        /// <summary>
        /// Indicates whether this game is associated with a daily challenge.
        /// This is a computed property not stored in the database.
        /// </summary>
        [NotMapped]
        public bool IsDailyChallenge => DailyChallangeId is not null;
    }
}
