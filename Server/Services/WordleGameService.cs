using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Models;
using Shared;

namespace Server.Services
{
    public class WordleGameService : IWordleGameService
    {
        private readonly WordleDbContext _context;

        public WordleGameService(WordleDbContext context)
        {
            _context = context;
        }

        public async Task<GameDto> GetOrCreateGameForDailyChallenge(int challengeId, string userId)
        {
            var gameDto = await GetChallangeGame(challengeId, userId);
            return gameDto ?? await CreateChallengeGame(challengeId, userId);
        }

        private async Task<GameDto?> GetChallangeGame(int challengeId, string userId)
        {
            return await _context.Games.Where(game => game.DailyChallangeId == challengeId && game.UserId == userId)
                .Select(game => new GameDto
                {
                    Id = game.Id,
                    IsWon = game.IsWon,
                    Attempts = game.Attempts.Select(a => new AttemptDto 
                    { 
                        Attempt = a.AttemptedWord, 
                        LettersState = CalculateLetterState(a)
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }

        private IEnumerable<State> CalculateLetterState(GameAttempt attempt)
        {
            var wordExpected = attempt.Game?.Word?.Text;
            var wordAttempted = attempt.AttemptedWord;
            
            if (wordExpected is null)
            {
                return new List<State>();
            }

            var result = new List<State>();
            for (int i = 0; i < wordAttempted.Length; i++)
            {
                if (wordAttempted[i] == wordExpected[i])
                {
                    result.Add(State.LetterGuessedCorrectPlace);
                }

                else if (wordExpected.Contains(wordAttempted[i]))
                {
                    result.Add(State.LetterGuessedIncorrectPlace);
                }
                else
                {
                    result.Add(State.LetterNotGuessed);
                }
            }

            return result;
        }

        private async Task<GameDto> CreateChallengeGame(int challengeId, string userId)
        {
            var user = await _context.Users.FirstAsync(u => u.Id == userId);
            var dailyChallenge = await _context.DailyChallenges.FirstAsync(dc => dc.Id == challengeId);
            var game = new Game { User = user, UserId = userId, DailyChallenge = dailyChallenge, DailyChallangeId = challengeId, WordId = dailyChallenge.WordId };
            
            _context.Games.Add(game);
            _context.SaveChanges();
            
            var gameDto = new GameDto
            {
                Id = game.Id,
                IsWon = game.IsWon,
                Attempts = new List<AttemptDto>()
            };

            return gameDto;
        }
    }
}
