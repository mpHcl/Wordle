using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public enum State
    {
        LetterGuessedCorrectPlace, 
        LetterGuessedIncorrectPlace,
        LetterNotGuessed
    }
    public class AttemptDto {
        public string Attempt { get; set; } = string.Empty;
        public IEnumerable<State> LettersState { get; set; } = new List<State>();

    }
    public class GameDto
    {
        public int Id { get; set; }
        public bool IsWon { get; set; }
        public IEnumerable<AttemptDto> Attempts { get; set; } = new List<AttemptDto>();
    }
}
