
using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Models;
using Server.Services.Interfaces;
using Shared.Dtos;
using SQLitePCL;

namespace Server.Services {
    public class LeaderboardService(WordleDbContext context) : ILeaderboardService {
        public async Task AddLostGame(string userId) {
            var leaderboardEntry = await context.Leaderboards.Where(l => l.UserId == userId).FirstOrDefaultAsync();

            if (leaderboardEntry is null) {
                leaderboardEntry = new Leaderboard {
                    UserId = userId,
                };
                context.Leaderboards.Add(leaderboardEntry);
            }
   
            leaderboardEntry.GamesPlayed += 1;
            leaderboardEntry.WinPercentage = (double)leaderboardEntry.GamesWon / leaderboardEntry.GamesPlayed * 100;
            leaderboardEntry.AverageGuesses = await CountGuessesAverage(userId);

            await context.SaveChangesAsync();
        }
        public async Task AddWonGame(string userId, bool hardMode, bool hints) {
            var leaderboardEntry = await context.Leaderboards.Where(l => l.UserId == userId).FirstOrDefaultAsync();

            if (leaderboardEntry is null) {
                leaderboardEntry = new Leaderboard {
                    UserId = userId,
                };
                context.Leaderboards.Add(leaderboardEntry);
            }

            leaderboardEntry.GamesPlayed += 1;
            leaderboardEntry.GamesWon += 1;
            leaderboardEntry.Points += hardMode && !hints ? 20 :
                                       hardMode ? 15 :
                                       !hints ? 10 :
                                       5;
            leaderboardEntry.WinPercentage = (double)leaderboardEntry.GamesWon / leaderboardEntry.GamesPlayed * 100;
            leaderboardEntry.AverageGuesses = await CountGuessesAverage(userId);

            await context.SaveChangesAsync();
        }

        private async Task<double> CountGuessesAverage(string userId) {
            var games = await context.Games.Where(g => g.UserId == userId && g.IsWon)
                .Include(g => g.Attempts)
                .Select(g => g.Attempts.Count)
                .ToListAsync();
            
            
            if (games is null || games.Count == 0) {
                return 0.0;
            }

            return games.Average();         
        }

        public async Task<List<LeaderboardEntryDto>> GetLeaderboard(int page, int pageSize) {
            var leaderboard = context.Leaderboards.Include(l => l.User)
                .OrderByDescending(l => l.Points)
                .ThenByDescending(l => l.WinPercentage)
                .ThenBy(l => l.AverageGuesses)
                .Select(l => new LeaderboardEntryDto {
                    Username = l.User!.UserName ?? "Anonymous",
                    GamesPlayed = l.GamesPlayed,
                    GamesWon = l.GamesWon,
                    WinPercentage = l.WinPercentage,
                    AverageGuesses = l.AverageGuesses,
                    Points = l.Points
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return await leaderboard.ToListAsync();
        }

        public async Task<List<LeaderboardEntryDto>> GetLeaderboard(int page, int pageSize, string filter) {
            var leaderboard = context.Leaderboards.Include(l => l.User)
                .Where(l => l.User!.UserName != null && l.User.UserName.Contains(filter))
                .OrderByDescending(l => l.Points)
                .ThenByDescending(l => l.WinPercentage)
                .ThenBy(l => l.AverageGuesses)
                .Select(l => new LeaderboardEntryDto {
                    Username = l.User!.UserName ?? "Anonymous",
                    GamesPlayed = l.GamesPlayed,
                    GamesWon = l.GamesWon,
                    WinPercentage = l.WinPercentage,
                    AverageGuesses = l.AverageGuesses,
                    Points = l.Points
                })
                .Skip((page - 1) * pageSize)
                .Take(pageSize);

            return await leaderboard.ToListAsync();
        }
    }
}
