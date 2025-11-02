namespace Server.Models
{
    public class Word
    {
        public int Id { get; set; }
        public required string Text { get; set; }
        public int CategoryId { get; set; }
        public WordCategory Category { get; set; } = null!;
    }
}
