using Shared.Dtos;

namespace Server.Services.Interfaces {
    /// <summary>
    /// Manages Wordle game creation, gameplay, and retrieval operations.
    /// </summary>
    public interface IWordleGameService {
        /// <summary>
        /// Retrieves an existing game for the specified daily challenge, or creates a new one if none exists.
        /// Daily challenge games are always played without hard mode or hints.
        /// </summary>
        /// <param name="challengeId">The ID of the daily challenge.</param>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The game information for the daily challenge.</returns>
        Task<GameDto> GetOrCreateGameForDailyChallenge(int challengeId, string userId);

        /// <summary>
        /// Creates a new game with a randomly selected word using the user's current settings (hard mode, hints).
        /// </summary>
        /// <param name="userId">The ID of the user starting the game.</param>
        /// <returns>The newly created game information.</returns>
        /// <exception cref="InvalidOperationException">Thrown when no words are available in the database.</exception>
        Task<GameDto> CreateNewGameAsync(string userId);

        /// <summary>
        /// Submits a word attempt for the specified game and updates game state, achievements, and leaderboard.
        /// </summary>
        /// <param name="gameId">The ID of the game.</param>
        /// <param name="attempt">The word being attempted (case-insensitive).</param>
        /// <param name="userId">The ID of the user making the attempt.</param>
        /// <returns>The updated game state including any newly earned achievements.</returns>
        /// <exception cref="ObjectNotFoundException">Thrown when the game is not found for the user.</exception>
        /// <exception cref="InvalidWordAttemptException">Thrown when the attempted word is not valid.</exception>
        /// <exception cref="GameAlreadyFinishedException">Thrown when attempting to play a finished game.</exception>
        Task<GameDto> MakeAttempt(int gameId, string attempt, string userId);

        /// <summary>
        /// Retrieves a list of the user's games, ordered by most recent first.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="skip">The number of games to skip for pagination.</param>
        /// <param name="pageSize">The maximum number of games to return.</param>
        /// <returns>A collection of the user's games.</returns>
        Task<IEnumerable<GameDto>> GetUserGamesAsync(string userId, int skip, int pageSize);

        /// <summary>
        /// Retrieves a specific game by its ID for the given user.
        /// </summary>
        /// <param name="userId">The ID of the user who owns the game.</param>
        /// <param name="gameId">The ID of the game to retrieve.</param>
        /// <returns>The game information, or null if not found.</returns>
        Task<GameDto?> GetGameByIdAsync(string userId, int gameId);
    }
}