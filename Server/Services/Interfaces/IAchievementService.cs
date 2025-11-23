using Shared.Dtos;

namespace Server.Services.Interfaces {
    /// <summary>
    /// Manages achievement tracking and retrieval for users in the Wordle game.
    /// </summary
    public interface IAchievementService {
        /// <summary>
        /// Checks for and awards new achievements earned by the user.
        /// </summary>
        /// <param name="userId">The ID of the user to check achievements for.</param>
        /// <returns>A collection of newly earned achievements.</returns>
        Task<ICollection<AchievementDto>> UpdateAchievements(string userId);

        /// <summary>
        /// Retrieves detailed information about a specific achievement, including how many users have earned it.
        /// </summary>
        /// <param name="achievementId">The ID of the achievement.</param>
        /// <returns>The achievement details with percentage of users who have earned it.</returns>
        /// <exception cref="ObjectNotFoundException">Thrown when the achievement is not found.</exception>
        Task<AchievementDto> GetAchievementDetails(int achievementId);

        /// <summary>
        /// Retrieves all available achievements in the system.
        /// </summary>
        /// <returns>A list of all achievements without user-specific data.</returns>
        Task<IEnumerable<AchievementDto>> GetAchievementsList();

        /// <summary>
        /// Retrieves all achievements with user-specific completion status.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A list of all achievements indicating which ones the user has earned.</returns>
        Task<IEnumerable<AchievementDto>> GetAchievementsListForUser(string userId);
    }
}