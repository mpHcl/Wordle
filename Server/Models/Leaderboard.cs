namespace Server.Models {
    public class Leaderboard {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public WordleUser? User { get; set; }
        public int GamesPlayed { get; set; } = 0;
        public int GamesWon { get; set; } = 0;
        public double WinPercentage { get; set; } = 0.0;
        public double AverageGuesses { get; set; } = 0.0;
    }
}
