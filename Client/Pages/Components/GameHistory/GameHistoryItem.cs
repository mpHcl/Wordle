namespace Client.Pages.Components.GameHistory {
    public class GameHistoryItem {
        public bool Won { get; set; }
        public string GuessedWord { get; set; } = string.Empty;
        public int Tries { get; set; }
        public DateTime Date { get; set; }
        public string Category { get; set; } = string.Empty;
    }
}
