
using Shared.Dtos;

namespace Server.Services {
    public interface ILeaderboardService {
        Task AddLostGame(string userId);
        Task AddWonGame(string userId, bool hardMode, bool hints);
        Task<List<LeaderboardEntryDto>> GetLeaderboard(int page, int pageSize);
    }
}
