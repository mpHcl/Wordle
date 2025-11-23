namespace Server.Models {
    /// <summary>
    /// Represents user-specific configuration settings for the Wordle game.
    /// Stores visual and gameplay preferences such as display mode and difficulty options.
    /// </summary>
    /// <remarks>
    /// This model is persisted to the database and associated with a user profile.
    /// All properties represent user preferences and can be modified at any time.
    /// </remarks>
    public class WordleSettings {
        /// <summary>
        /// Primary key for the settings record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Indicates whether dark mode is enabled.
        /// When enabled, the game's UI uses a darker color palette.
        /// </summary>
        public bool DarkMode { get; set; } = false;

        /// <summary>
        /// Indicates whether hard mode is enabled.
        /// Hard mode indicates that only letters in correct places will be 
        /// highlited. 
        /// </summary>
        public bool HardMode { get; set; } = false;

        /// <summary>
        /// Indicates whether high contrast mode is enabled.
        /// When enabled, UI colors are adjusted to improve accessibility.
        /// </summary>
        public bool HighContrastMode { get; set; } = false;

        /// <summary>
        /// Indicates whether in-game hints are shown to the player.
        /// </summary>
        public bool ShowHints { get; set; } = true;
    }
}
