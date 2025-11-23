using Shared.Dtos;

namespace Server.Services.Interfaces {
    /// <summary>
    /// Manages user-specific game settings for Wordle.
    /// Settings are created during user registration and can only be retrieved or updated through this service.
    /// </summary>
    public interface IWordleSettingsService {
        /// <summary>
        /// Retrieves the current settings for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>The user's current settings.</returns>
        /// <exception cref="ObjectNotFoundException">Thrown when the user or their settings are not found.</exception>
        Task<SettingsDto> GetSettingsForUser(string userId);

        /// <summary>
        /// Updates the settings for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="settings">The new settings to apply.</param>
        /// <exception cref="ObjectNotFoundException">Thrown when the user or their settings are not found.</exception>
        Task UpdateSettingsForUser(string userId, SettingsDto settings);
    }
}