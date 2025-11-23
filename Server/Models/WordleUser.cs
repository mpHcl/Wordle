using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models {
    /// <summary>
    /// Represents an application user participating in the Wordle game.
    /// Extends <see cref="IdentityUser"/> and stores additional game-related
    /// data such as user settings, achievements, and game history.
    /// </summary>
    /// <remarks>
    /// This model is persisted to the database as part of ASP.NET Identity.
    /// Navigation properties such as <see cref="Settings"/>, <see cref="UserAchievements"/>,
    /// and <see cref="Games"/> can be loaded by Entity Framework using the <c>Include</c> method.
    /// </remarks>
    public class WordleUser : IdentityUser {
        /// <summary>
        /// Foreign key referencing the associated <see cref="WordleSettings"/> entity.
        /// Defines a one-to-one relationship between the user and their settings.
        /// </summary>
        public int SettingsId { get; set; }

        /// <summary>
        /// Navigation property containing the user's configuration settings for the game,
        /// including display modes and gameplay preferences.
        /// </summary>
        public WordleSettings Settings { get; set; } = new WordleSettings();

        /// <summary>
        /// Collection of achievement records associated with the user.
        /// Each entry represents a specific unlocked achievement.
        /// </summary>
        public ICollection<UserAchievements> UserAchievements { get; set; } = [];

        /// <summary>
        /// Collection of games played by the user.
        /// Each game entry contains full information about gameplay results and settings.
        /// </summary>
        public ICollection<Game> Games { get; set; } = [];

        /// <summary>
        /// Computed collection of achievements unlocked by the user.
        /// This property maps <see cref="UserAchievements"/> to their corresponding
        /// <see cref="Achievement"/> entries and is not persisted to the database.
        /// </summary>
        [NotMapped]
        public IEnumerable<Achievement?>? Achievements => UserAchievements?.Select(ua => ua.Achievement);
    }
}
