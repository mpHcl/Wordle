namespace Server.Models {
    /// <summary>
    /// Represents a record of an achievement unlocked by a specific user.
    /// Links the user with an achievement and stores the date it was obtained.
    /// </summary>
    /// <remarks>
    /// This model is persisted to the database and acts as a junction table 
    /// in a many-to-many relationship between <see cref="WordleUser"/> and 
    /// <see cref="Achievement"/>.
    /// Navigation properties <see cref="User"/> and <see cref="Achievement"/> 
    /// can be included in queries using the <c>Include</c> method.
    /// </remarks>
    public class UserAchievements {
        /// <summary>
        /// Foreign key referencing the user who earned the achievement.
        /// </summary>
        public required string UserId { get; set; }

        /// <summary>
        /// Navigation property referencing the user who earned the achievement.
        /// </summary>
        public WordleUser? User { get; set; }

        /// <summary>
        /// Foreign key referencing the unlocked <see cref="Achievement"/>.
        /// </summary>
        public int AchievementId { get; set; }

        /// <summary>
        /// Navigation property representing the achievement unlocked by the user.
        /// </summary>
        public Achievement? Achievement { get; set; }

        /// <summary>
        /// The date and time when the achievement was obtained.
        /// Stored in UTC.
        /// </summary>
        public DateTime DateAchieved { get; set; } = DateTime.UtcNow;
    }
}
