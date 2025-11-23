namespace Server.Models {
    /// <summary>
    /// Represents a daily challenge informations. Contains a single word 
    /// assigned to a specific date.
    /// </summary>
    /// <remarks>
    /// This model is persisted to the database. 
    /// The <see cref="Date"/> and <see cref="WordId"/>
    /// properties are required and should be provided when creating a new daily challenge.
    /// The <see cref="Word"/> is automatically provided by EF, to use it in queries
    /// use <c>Include</c> method. 
    /// </remarks>
    public class DailyChallenge {
        /// <summary>
        /// Primary key for the DailyChallenge.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Date of the challenge. Each challenge corresponds to a unique date.
        /// </summary>
        public required DateOnly Date { get; set; }


        /// <summary>
        /// Foreign key referencing the related <see cref="Word"/> entity.
        /// </summary>
        public required int WordId { get; set; }

        /// <summary>
        /// Navigation property representing the word associated with this challenge.
        /// This property is populated by Entity Framework when performing queries that include
        /// the related <see cref="Word"/> via <c>.Include(x => x.Word)</c>.
        /// </summary>
        public Word? Word { get; set; }
    }
}
