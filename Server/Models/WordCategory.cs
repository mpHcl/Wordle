namespace Server.Models {
    public class WordCategory {
        public int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }

        public ICollection<Word> Words { get; set; } = new List<Word>();
    }
}
