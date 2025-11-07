using Shared.Dtos;

namespace Server.Services
{
    public interface IDailyChallengeService
    {
        Task<DailyChallengeDto> CreateToday();
        Task<DailyChallengeDto?> GetToday();
    }
}