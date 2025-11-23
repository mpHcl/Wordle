namespace Server.Models {
    /// <summary>
    /// Represents a single achievement that can be earned by a user.
    /// </summary>
    /// <remarks>
    /// This model is persisted to the database. 
    /// The <see cref="Name"/> and <see cref="Description"/>
    /// properties are required and should be provided when creating a new achievement.
    /// </remarks>
    public class Achievement {
        /// <summary>
        /// Primary key for the achievement.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Human-readable, short name of the achievement.
        /// </summary>
        /// <example>First Win</example>
        public required string Name { get; set; }

        /// <summary>
        /// Longer description that explains how the achievement is earned.
        /// </summary>
        /// <example>Win a game for the first time.</example>
        public required string Description { get; set; }
    }
}
