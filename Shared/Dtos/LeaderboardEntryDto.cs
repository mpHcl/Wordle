namespace Shared.Dtos {
    public class LeaderboardEntryDto {
        public required string Username { get; set; }
        public required int GamesPlayed { get; set; }
        public required int GamesWon { get; set; }
        public required double WinPercentage { get; set; }
        public required double AverageGuesses { get; set; }
        public required double Points { get; set; }
    }
}