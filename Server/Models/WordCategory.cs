namespace Server.Models {
    /// <summary>
    /// Represents categories of the words. Contains name, Description and, 
    /// collection of words.
    /// </summary>
    /// <remarks>
    /// This model is persisted to the database. 
    /// The <see cref="Name"/> and <see cref="Description"/>
    /// properties are required and should be provided when creating a new achievement.
    /// The <see cref="Words"/> collection can be loaded by Entity Framework using the <c>Include</c> method.y
    public class WordCategory {
        /// <summary>
        /// Primary key for the WordCategory.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Name of the category. 
        /// </summary>
        public required string Name { get; set; }

        /// <summary>
        /// Description of the category. 
        /// </summary>
        public required string Description { get; set; }


        /// <summary>
        /// Collection of word in the category.
        /// </summary>
        public ICollection<Word> Words { get; set; } = new List<Word>();
    }
}
