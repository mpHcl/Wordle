using Shared.Dtos;

namespace Server.Services.Interfaces {
    /// <summary>
    /// Manages daily word challenges for the Wordle game.
    /// </summary>
    public interface IDailyChallengeService {
        /// <summary>
        /// Creates a new daily challenge for today with a randomly selected word.
        /// If a challenge already exists for today, returns the existing challenge.
        /// </summary>
        /// <returns>The daily challenge information containing the challenge ID.</returns>
        Task<DailyChallengeDto> CreateToday();

        /// <summary>
        /// Retrieves today's daily challenge if it exists.
        /// </summary>
        /// <returns>The daily challenge information, or null if no challenge exists for today.</returns>
        Task<DailyChallengeDto?> GetToday();
    }
}