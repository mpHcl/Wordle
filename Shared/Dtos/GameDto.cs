using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Dtos {
    /// <summary>
    /// Indicates the state of an individual letter in a word attempt.
    /// </summary>
    public enum State {
        /// <summary>The letter is in the correct position.</summary>
        LetterGuessedCorrectPlace,

        /// <summary>The letter exists in the word but is in the wrong position.</summary>
        LetterGuessedIncorrectPlace,

        /// <summary>The letter does not appear in the target word.</summary>
        LetterNotGuessed
    }

    /// <summary>
    /// Represents the status of a word-guessing game.
    /// </summary>
    public enum GameStatus {
        /// <summary>The game was won by the player.</summary>
        Won,

        /// <summary>The game ended but was not won.</summary>
        Finished,

        /// <summary>The game is still in progress.</summary>
        InProgress
    }

    /// <summary>
    /// Represents a single attempt made by the user during a game.
    /// </summary>
    public class AttemptDto {
        /// <summary>
        /// The attempted word entered by the user.
        /// </summary>
        public string Attempt { get; set; } = string.Empty;

        /// <summary>
        /// The evaluation result for each letter in the attempted word.
        /// </summary>
        public List<State> LettersState { get; set; } = [];

    }

    /// <summary>
    /// Complete game information including attempts and results.
    /// </summary>
    public class GameDto {
        /// <summary>
        /// Unique identifier of the game session.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Current status of the game.
        /// </summary>
        public GameStatus GameStatus { get; set; }

        /// <summary>
        /// List of attempts made during the game.
        /// </summary>
        public List<AttemptDto> Attempts { get; set; } = [];

        /// <summary>
        /// Whether hard mode was enabled (stricter gameplay rules).
        /// </summary>
        public required bool HardMode { get; set; }

        /// <summary>
        /// Whether word hints were enabled.
        /// </summary>
        public required bool Hints { get; set; }

        /// <summary>
        /// The target word for the game (may be omitted for active games).
        /// </summary>
        public string? Word { get; set; }

        /// <summary>
        /// Category or theme of the target word, if applicable.
        /// </summary>
        public string? Category { get; set; }

        /// <summary>
        /// Achievements newly unlocked as a result of completing the game.
        /// </summary>
        public List<AchievementDto>? NewAchievements { get; set; }
     }
}
