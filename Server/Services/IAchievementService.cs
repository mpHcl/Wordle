using Shared.Dtos;

namespace Server.Services {
    public interface IAchievementService {
        Task<IEnumerable<AchievementDto>> UpdateAchievements(string userId);
        Task<AchievementDto> GetAchievementDetails(int achievementId);
        Task<IEnumerable<AchievementDto>> GetAchievementsList();
        Task<IEnumerable<AchievementDto>> GetAchievementsListForUser(string userId);
    }
}