using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Models;
using Shared.Dtos;

namespace Server.Services {
    public class AchievementService(UserManager<WordleUser> userManager, WordleDbContext context) 
            : IAchievementService {

        private readonly WordleDbContext _context = context;
        private readonly UserManager<WordleUser> _userManager = userManager;
        
        public async Task<AchievementDto> GetAchievementDetails(int achievementId) {
            var achievement = await _context.Achievements.FindAsync(achievementId) 
                ?? throw new Exception("Not found");
            var num_of_achievements = await _context.UserAchievements
                                                        .Where(a => a.AchievementId == achievementId)
                                                        .CountAsync();

            var num_of_users = await _userManager.Users.CountAsync();
            var percent = 100.0 * (double) num_of_achievements / (double) num_of_users;

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

        public async Task<IEnumerable<AchievementDto>> UpdateAchievements(string userId) {
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
                    case "First Solve!": CheckFirstSolveAchievement(newAchievements, userId); break;
                    case "First Try!": await CheckFirstTryAchievement(newAchievements, userId); break;
                    case "Persistent Player": CheckPersistentPlayerAchievement(newAchievements, userId); break;
                    case "Category Master": CheckCategoryMasterAchievement(newAchievements, userId); break;
                    case "No hints": CheckNoHintsAchievement(newAchievements, userId); break;
                    case "Hard mode!": CheckHardModeAchievement(newAchievements, userId); break;
                }
            }

            return newAchievements;
        }

        private void CheckHardModeAchievement(List<AchievementDto> newAchievements, string userId) {
            throw new NotImplementedException();
        }

        private void CheckNoHintsAchievement(List<AchievementDto> newAchievements, string userId) {
            throw new NotImplementedException();
        }

        private void CheckCategoryMasterAchievement(List<AchievementDto> newAchievements, string userId) {
            throw new NotImplementedException();
        }

        private void CheckPersistentPlayerAchievement(List<AchievementDto> newAchievements, string userId) {
            throw new NotImplementedException();
        }

        private async Task CheckFirstTryAchievement(List<AchievementDto> newAchievements, string userId) {
            var game = await _context.Games.Where(g => g.UserId == userId && g.IsWon && g.NumebrOfAttempts == 1)
                .FirstOrDefaultAsync();

            if (game is null) {
                return;
            }

            var achievement = new UserAchievements { UserId = userId, AchievementId = 2, DateAchieved = DateTime.UtcNow.Date };

            await _context.AddAsync(achievement);
            await _context.SaveChangesAsync();

            var achievementDto = await GetAchievementDetails(2);
            newAchievements.Add(achievementDto);
        }

        private void CheckFirstSolveAchievement(List<AchievementDto> newAchievements, string userId) {
            throw new NotImplementedException();
        }
    }
}
