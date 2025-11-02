namespace Server.Models
{
    public class DailyChallenge
    {
        public int Id { get; set; }
        public required DateOnly Date { get; set; }
        public required int WordId { get; set; }
        public Word? Word { get; set; }
    }
}
