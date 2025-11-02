using System.ComponentModel.DataAnnotations.Schema;

namespace Server.Models
{
    public class Game
    {
        public int Id { get; set; }
        public required string UserId { get; set; }
        public WordleUser? User { get; set; }
        public required int WordId { get; set; }
        public Word? Word { get; set; }

        public bool IsWon { get; set; } = false;

        public ICollection<GameAttempt> Attempts { get; set; } = new List<GameAttempt>();
        
        [NotMapped]
        public int NumebrOfAttempts => Attempts.Count();
    }
}
