namespace Server.Models {
    public class GameAttempt {
        public int Id { get; set; }
        public required int GameId { get; set; }
        public Game? Game { get; set; }
        public required string AttemptedWord { get; set; }
        public required DateTime AttemptedAt { get; set; }
    }
}
