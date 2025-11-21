using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Exceptions;
using Server.Models;
using Server.Services.Interfaces;
using Shared.Dtos;

namespace Server.Services {
    public class AchievementService(UserManager<WordleUser> userManager, WordleDbContext context)
            : IAchievementService {

        private readonly WordleDbContext _context = context 
            ?? throw new ArgumentNullException(nameof(context));
        private readonly UserManager<WordleUser> _userManager = userManager 
            ?? throw new ArgumentNullException(nameof(userManager));

        public async Task<AchievementDto> GetAchievementDetails(int achievementId) {
            var achievement = await _context.Achievements.FindAsync(achievementId)
                ?? throw new ObjectNotFoundException($"Achievement {achievementId} not found.");
            var num_of_achievements = await _context.UserAchievements
                                                        .Where(a => a.AchievementId == achievementId)
                                                        .CountAsync();

            var num_of_users = await _userManager.Users.CountAsync();
            var percent = 100.0 * (double)num_of_achievements / (double)num_of_users;

            return new AchievementDto {
                Id = achievementId,
                Name = achievement.Name,
                Description = achievement.Description,
                PercentOfUsers = percent
            };
        }

        public async Task<IEnumerable<AchievementDto>> GetAchievementsList() {
            return await _context.Achievements
                .Select(a => new AchievementDto {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<AchievementDto>> GetAchievementsListForUser(string userId) {
            return await _context.Achievements
                .Select(a => new AchievementDto {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Achieved = _context.UserAchievements
                            .Where(ua => ua.UserId == userId)
                            .Select(ua => ua.AchievementId)
                            .Contains(a.Id)
                })
                .ToListAsync();
        }

        public async Task<ICollection<AchievementDto>> UpdateAchievements(string userId) {
            var achievementsToCheck = await _context.Achievements.Where(a =>
                !_context.UserAchievements
                    .Where(ua => ua.UserId == userId)
                    .Select(ua => ua.AchievementId)
                    .Contains(a.Id)
            )
            .AsNoTracking()
            .ToListAsync();

            var newAchievements = new List<AchievementDto>();
            foreach (var achievement in achievementsToCheck) {
                switch (achievement.Name) {
                    case "First Solve!": await CheckFirstSolveAchievement(userId, newAchievements); break;
                    case "First Try!": await CheckFirstTryAchievement(userId, newAchievements); break;
                    case "Persistent Player": await CheckPersistentPlayerAchievement(userId, newAchievements); break;
                    case "Category Master": await CheckCategoryMasterAchievement(userId, newAchievements); break;
                    case "No hints": await CheckNoHintsAchievement(userId, newAchievements); break;
                    case "Hard mode!": await CheckHardModeAchievement(userId, newAchievements); break;
                }
            }

            return newAchievements;
        }

        private async Task CheckHardModeAchievement(string userId, List<AchievementDto> newAchievements) {
            var game = await _context.Games
                .Include(g => g.Attempts)
                .Where(g => g.UserId == userId && g.IsWon && g.HardMode)
                .AnyAsync();

            if (!game) {
                return;
            }

            var achievement = new UserAchievements { UserId = userId, AchievementId = 6, DateAchieved = DateTime.UtcNow.Date };

            await _context.AddAsync(achievement);
            await _context.SaveChangesAsync();

            var achievementDto = await GetAchievementDetails(6);
            newAchievements.Add(achievementDto);
        }

        private async Task CheckNoHintsAchievement(string userId, List<AchievementDto> newAchievements) {
            var game = await _context.Games
                .Include(g => g.Attempts)
                .Where(g => g.UserId == userId && g.IsWon && !g.Hints)
                .AnyAsync();

            if (!game) {
                return;
            }

            var achievement = new UserAchievements { UserId = userId, AchievementId = 5, DateAchieved = DateTime.UtcNow.Date };

            await _context.AddAsync(achievement);
            await _context.SaveChangesAsync();

            var achievementDto = await GetAchievementDetails(5);
            newAchievements.Add(achievementDto);
        }

        private async Task CheckCategoryMasterAchievement(string userId, List<AchievementDto> newAchievements) {
            var count = await _context.Games
                .Where(g => g.UserId == userId && g.IsWon)
                .Include(g => g.Word)
                .GroupBy(g => g.Word!.CategoryId)
                .Where(g => g.Count() >= 5)
                .CountAsync();

            var numOfCategories = await _context.WordCategories.CountAsync();

            if (count != numOfCategories) {
                return;
            }

            var achievement = new UserAchievements { UserId = userId, AchievementId = 4, DateAchieved = DateTime.UtcNow.Date };

            await _context.AddAsync(achievement);
            await _context.SaveChangesAsync();

            var achievementDto = await GetAchievementDetails(4);
            newAchievements.Add(achievementDto);
        }

        private async Task CheckPersistentPlayerAchievement(string userId, List<AchievementDto> newAchievements) {
            var dates = await _context.GameAttempts
                .Include(a => a.Game)
                .Where(a => a.Game!.UserId == userId)
                .Select(a => a.AttemptedAt.Date)
                .Distinct()
                .OrderBy(d => d)
                .ToListAsync();

            var has7DayStreak = dates
                .Select((date, index) => new { date, index })
                .GroupBy(x => x.date.AddDays(-x.index))
                .Any(g => g.Count() >= 7);


            if (!has7DayStreak) {
                return;
            }

            var achievement = new UserAchievements { UserId = userId, AchievementId = 3, DateAchieved = DateTime.UtcNow.Date };

            await _context.AddAsync(achievement);
            await _context.SaveChangesAsync();

            var achievementDto = await GetAchievementDetails(3);
            newAchievements.Add(achievementDto);
        }

        private async Task CheckFirstTryAchievement(string userId, List<AchievementDto> newAchievements) {
            var game = await _context.Games
                .Include(g => g.Attempts)
                .Where(g => g.UserId == userId && g.IsWon && g.Attempts.Count == 1)
                .AnyAsync();

            if (!game) {
                return;
            }

            var achievement = new UserAchievements { UserId = userId, AchievementId = 2, DateAchieved = DateTime.UtcNow.Date };

            await _context.AddAsync(achievement);
            await _context.SaveChangesAsync();

            var achievementDto = await GetAchievementDetails(2);
            newAchievements.Add(achievementDto);
        }

        private async Task CheckFirstSolveAchievement(string userId, List<AchievementDto> newAchievements) {
            var game = await _context.Games
                .Include(g => g.Attempts)
                .Where(g => g.UserId == userId && g.IsWon)
                .AnyAsync();

            if (!game) {
                return;
            }

            var achievement = new UserAchievements { UserId = userId, AchievementId = 1, DateAchieved = DateTime.UtcNow.Date };

            await _context.AddAsync(achievement);
            await _context.SaveChangesAsync();

            var achievementDto = await GetAchievementDetails(1);
            newAchievements.Add(achievementDto);
        }
    }
}
