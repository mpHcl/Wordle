namespace Server.Models {
    /// <summary>
    /// Represents aggregated statistical data for a user, used to display
    /// their position on the leaderboard.
    /// Contains gameplay performance metrics such as win rate, average guesses,
    /// and total points.
    /// </summary>
    /// <remarks>
    /// This model is persisted to the database. 
    /// The <see cref="UserId"/> property is required and associates the leaderboard entry 
    /// with a specific <see cref="WordleUser"/>.
    /// Navigation property <see cref="User"/> is populated by Entity Framework when included 
    /// explicitly in queries using the <c>Include</c> method.
    /// </remarks>
    public class Leaderboard {
        /// <summary>
        /// Primary key for the leaderboard entry.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key referencing the user associated with this leaderboard record.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Navigation property for the user whose statistics are represented.
        /// </summary>
        public WordleUser? User { get; set; }

        /// <summary>
        /// Total number of games played by the user.
        /// </summary>
        public int GamesPlayed { get; set; } = 0;

        /// <summary>
        /// Total number of games won by the user.
        /// </summary>
        public int GamesWon { get; set; } = 0;

        /// <summary>
        /// Percentage of games the user has won.
        /// Calculated as <c>(GamesWon / GamesPlayed) * 100</c> when GamesPlayed &gt; 0.
        /// </summary>
        public double WinPercentage { get; set; } = 0.0;

        /// <summary>
        /// The average number of guesses needed for the user to complete a game.
        /// </summary>
        public double AverageGuesses { get; set; } = 0.0;

        /// <summary>
        /// Total points accumulated by the user based on performance.
        /// </summary>
        public int Points { get; set; } = 0;
    }
}
