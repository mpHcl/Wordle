using Shared.Dtos;

namespace Server.Services.Interfaces {
    public interface IDailyChallengeService {
        Task<DailyChallengeDto> CreateToday();
        Task<DailyChallengeDto?> GetToday();
    }
}