
using Shared;

namespace Server.Services
{
    public interface IWordleGameService
    {
        Task<GameDto> GetOrCreateGameForDailyChallenge(int challengeId, string userId);
    }
}