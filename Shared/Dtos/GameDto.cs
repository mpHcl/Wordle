using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos {
    public enum State {
        LetterGuessedCorrectPlace,
        LetterGuessedIncorrectPlace,
        LetterNotGuessed
    }

    public enum GameStatus {
        Won,
        Finished,
        InProgress
    }
    public class AttemptDto {
        public string Attempt { get; set; } = string.Empty;
        public List<State> LettersState { get; set; } = [];

    }
    public class GameDto {
        public int Id { get; set; }
        public GameStatus GameStatus { get; set; }
        public List<AttemptDto> Attempts { get; set; } = [];

        public required bool HardMode { get; set; }
        public required bool Hints { get; set; }
        public string? Word { get; set; }
        public string? Category { get; set; }
        public List<AchievementDto>? NewAchievements { get; set; }
     }
}
