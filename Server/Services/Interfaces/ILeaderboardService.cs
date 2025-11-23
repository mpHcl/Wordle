using Shared.Dtos;

namespace Server.Services.Interfaces {
    /// <summary>
    /// Manages leaderboard statistics and rankings for Wordle players.
    /// </summary>
    public interface ILeaderboardService {
        /// <summary>
        /// Records a lost game for the user and updates their leaderboard statistics.
        /// </summary>
        /// <param name="userId">The ID of the user who lost the game.</param>
        Task AddLostGame(string userId);

        /// <summary>
        /// Records a won game for the user and updates their leaderboard statistics with points based on game difficulty.
        /// Points awarded: 20 (hard mode + no hints), 15 (hard mode), 10 (no hints), or 5 (standard).
        /// </summary>
        /// <param name="userId">The ID of the user who won the game.</param>
        /// <param name="hardMode">Whether the game was played in hard mode.</param>
        /// <param name="hints">Whether hints were used during the game.</param>
        Task AddWonGame(string userId, bool hardMode, bool hints);

        /// <summary>
        /// Retrieves a leaderboard ordered by points, win percentage, and average guesses.
        /// </summary>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The number of entries per page.</param>
        /// <returns>A list of leaderboard entries for the requested page.</returns>
        Task<List<LeaderboardEntryDto>> GetLeaderboard(int page, int pageSize);

        /// <summary>
        /// Retrieves a filtered leaderboard matching the specified username filter.
        /// </summary>
        /// <param name="page">The page number (1-based).</param>
        /// <param name="pageSize">The number of entries per page.</param>
        /// <param name="filter">The username filter to apply (case-insensitive partial match).</param>
        /// <returns>A list of leaderboard entries matching the filter for the requested page.</returns>
        Task<List<LeaderboardEntryDto>> GetLeaderboard(int page, int pageSize, string filter);
    }
}
