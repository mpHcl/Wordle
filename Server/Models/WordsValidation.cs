namespace Server.Models {
    /// <summary>
    /// Database table that contains dictionary words that could be used
    /// to make an attempt in wordle game. Unlike Words table entries
    /// do not have assosiations to category, and cannot be used as 
    /// game word. 
    /// </summary>
    /// <remarks>
    /// Every words from <see cref="Word"/> model should be stored
    /// also in this table. The table is automatically populated with 
    /// content from WORDS.txt file. 
    /// </remarks>
    public class WordsValidation {
        /// <summary>
        /// Primary key for the WordsValidation.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Word as plain text. 
        /// </summary>
        public required string Text { get; set; }
    }
}
