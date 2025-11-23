namespace Server.Models {
    /// <summary>
    /// Represents a single word used in the Wordle game.
    /// Contains the word text and a reference to its assigned category.
    /// </summary>
    /// <remarks>
    /// This model is persisted to the database.
    /// The <see cref="Text"/> and <see cref="CategoryId"/> properties are required.
    /// The <see cref="Category"/> navigation property is populated by Entity Framework
    /// when explicitly included in queries using the <c>Include</c> method.
    /// </remarks>
    public class Word {
        /// <summary>
        /// Primary key for the word.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// The actual word text used in the game.
        /// Must be provided when creating a new word.
        /// </summary>
        public required string Text { get; set; }

        /// <summary>
        /// Foreign key referencing the <see cref="WordCategory"/> to which this word belongs.
        /// </summary>
        public int CategoryId { get; set; }

        /// <summary>
        /// Navigation property representing the category of the word.
        /// Loaded automatically by Entity Framework when included in queries.
        /// </summary>
        public WordCategory Category { get; set; } = null!;
    }
}
