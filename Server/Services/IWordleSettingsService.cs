using Shared.Dtos;

namespace Server.Services {
    public interface IWordleSettingsService {
        Task<SettingsDto> GetSettingsForUser(string userId);
        Task UpdateSettingsForUser(string userId, SettingsDto settings);
    }
}