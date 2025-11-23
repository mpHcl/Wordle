namespace Server.Models {
    /// <summary>
    /// Represents a game attempt to a specific game made by user.  
    /// </summary>
    /// <remarks>
    /// This model is persisted to the database. 
    /// The <see cref="GameId"/>, <see cref="AttemptedWord"/> and <see cref="AttemptedAt"/>
    /// properties are required and should be provided when creating a new daily challenge.
    /// The <see cref="Game"/> is automatically provided by EF, to use it in queries
    /// use <c>Include</c> method. 
    /// </remarks
    public class GameAttempt {
        /// <summary>
        /// Primary key for the DailyChallenge.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Foreign key referencing the related <see cref="Game"/> entity.
        /// </summary>
        public required int GameId { get; set; }

        /// <summary>
        /// Navigation property representing the game associated with this attempt.
        /// This property is populated by Entity Framework when performing queries that include
        /// the related <see cref="Models.Game"/> via <c>.Include(x => x.Game)</c>.
        /// </summary>
        public Game? Game { get; set; }

        /// <summary>
        /// Attempted word (plain text) made by user.
        /// </summary>
        public required string AttemptedWord { get; set; }

        /// <summary>
        /// Date of the attemp. 
        /// </summary>
        public required DateTime AttemptedAt { get; set; }
    }
}
