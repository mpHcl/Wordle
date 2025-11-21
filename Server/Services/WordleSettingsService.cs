using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Exceptions;
using Server.Models;
using Server.Services.Interfaces;
using Shared.Dtos;

namespace Server.Services {
    public class WordleSettingsService(UserManager<WordleUser> userManager, WordleDbContext context) 
        : IWordleSettingsService {

        private readonly UserManager<WordleUser> _userManager = userManager 
            ?? throw new ArgumentNullException(nameof(userManager));
        private readonly WordleDbContext _context = context
            ?? throw new ArgumentNullException(nameof(context));


        public async Task<SettingsDto> GetSettingsForUser(string userId) {
            var user = await _userManager.Users
                .Include(u => u.Settings)
                .FirstOrDefaultAsync(u => u.Id == userId) 
                ?? throw new ObjectNotFoundException("User not found");

            if (user.Settings is null) {
                throw new ObjectNotFoundException("User settings not found");
            }

            return new SettingsDto {
                DarkMode = user.Settings.DarkMode,
                HardMode = user.Settings.HardMode,
                HighContrastMode = user.Settings.HighContrastMode,
                ShowHints = user.Settings.ShowHints
            };
        }

        public async Task UpdateSettingsForUser(string userId, SettingsDto settings) {
            var user = _userManager.Users.Include(u => u.Settings).FirstOrDefault(u => u.Id == userId) 
                ?? throw new ObjectNotFoundException("User not found");

            if (user.Settings is null) {
                throw new ObjectNotFoundException("User settings not found");
            }

            user.Settings.HardMode = settings.HardMode;
            user.Settings.DarkMode = settings.DarkMode;
            user.Settings.HighContrastMode = settings.HighContrastMode;
            user.Settings.ShowHints = settings.ShowHints;
            
            await _context.SaveChangesAsync();
        }
    }
}
