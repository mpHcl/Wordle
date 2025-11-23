namespace Shared.Dtos {
    /// <summary>
    /// Represents a single leaderboard entry for a player.
    /// </summary>
    public class LeaderboardEntryDto {
        /// <summary>
        /// Username of the player.
        /// </summary>
        public required string Username { get; set; }

        /// <summary>
        /// Total number of games the user has played.
        /// </summary>
        public required int GamesPlayed { get; set; }

        /// <summary>
        /// Total number of games the user has won.
        /// </summary>
        public required int GamesWon { get; set; }

        /// <summary>
        /// Percentage of games the user has won.
        /// </summary>
        public required double WinPercentage { get; set; }

        /// <summary>
        /// Average number of guesses per completed game.
        /// </summary>
        public required double AverageGuesses { get; set; }

        /// <summary>
        /// Ranking score used to sort the leaderboard.
        /// </summary>
        public required double Points { get; set; }
    }
}