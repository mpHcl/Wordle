using Shared.Dtos;

namespace Server.Services.Interfaces {
    public interface IWordleGameService {
        Task<GameDto> GetOrCreateGameForDailyChallenge(int challengeId, string userId);
        Task<GameDto> CreateNewGameAsync(string userId);
        Task<GameDto> MakeAttempt(int gameId, string attempt, string userId);
        Task<IEnumerable<GameDto>> GetUserGamesAsync(string userId, int skip, int pageSize);
        Task<GameDto?> GetGameByIdAsync(string userId, int gameId);
    }
}