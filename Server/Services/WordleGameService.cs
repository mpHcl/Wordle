using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Exceptions;
using Server.Models;
using Shared;
using System;

namespace Server.Services {
    public class WordleGameService : IWordleGameService {
        private readonly WordleDbContext _context;
        private static readonly Random _random = new();

        public WordleGameService(WordleDbContext context) {
            _context = context;
        }

        private static List<State> CalculateLetterState(GameAttempt attempt) {
            var wordExpected = attempt.Game?.Word?.Text.ToUpper();
            var wordAttempted = attempt.AttemptedWord.ToUpper();

            if (wordExpected is null) {
                return new List<State>();
            }

            var result = new List<State>();
            for (int i = 0; i < wordAttempted.Length; i++) {
                if (wordAttempted[i] == wordExpected[i]) {
                    result.Add(State.LetterGuessedCorrectPlace);
                }
                else if (wordExpected.Contains(wordAttempted[i])) {
                    result.Add(State.LetterGuessedIncorrectPlace);
                }
                else {
                    result.Add(State.LetterNotGuessed);
                }
            }

            return result;
        }

        public async Task<GameDto> GetOrCreateGameForDailyChallenge(int challengeId, string userId) {
            var gameDto = await GetChallangeGame(challengeId, userId);
            return gameDto ?? await CreateChallengeGame(challengeId, userId);
        }

        private async Task<GameDto?> GetChallangeGame(int challengeId, string userId) {
            var game = await _context.Games
                .Where(game => game.DailyChallangeId == challengeId && game.UserId == userId)
                .Include(g => g.Attempts)
                .Include(g => g.Word)
                .FirstOrDefaultAsync();

            if (game is null) {
                return null;
            }

            var result = new GameDto() {
                Id = game.Id,
                GameStaus = CalculateStatus(game),
                Attempts = game.Attempts.Select(a => new AttemptDto {
                    Attempt = a.AttemptedWord,
                    LettersState = CalculateLetterState(a)
                }).ToList()
            };

            return result;
        }

        private async Task<GameDto> CreateChallengeGame(int challengeId, string userId) {
            var user = await _context.Users.FirstAsync(u => u.Id == userId);
            var dailyChallenge = await _context.DailyChallenges.FirstAsync(dc => dc.Id == challengeId);
            var game = new Game { User = user, UserId = userId, DailyChallenge = dailyChallenge, DailyChallangeId = challengeId, WordId = dailyChallenge.WordId };

            _context.Games.Add(game);
            _context.SaveChanges();

            var gameDto = new GameDto {
                Id = game.Id,
                GameStaus = CalculateStatus(game),
                Attempts = new List<AttemptDto>()
            };

            return gameDto;
        }

        public async Task<GameDto> CreateNewGame(string userId) {
            var user = await _context.Users.FirstAsync(u => u.Id == userId);
            var wordsIds = await _context.Words.Select(w => w.Id).ToListAsync();
            var randomWordId = wordsIds[_random.Next(0, wordsIds.Count)];
            var randomWord = await _context.Words.FirstAsync(w => w.Id == randomWordId);

            var game = new Game { User = user, UserId = userId, Word = randomWord, WordId = randomWord.Id };

            _context.Games.Add(game);
            _context.SaveChanges();

            var gameDto = new GameDto {
                Id = game.Id,
                GameStaus = CalculateStatus(game),
                Attempts = new List<AttemptDto>()
            };

            return gameDto;
        }

        private static GameStatus CalculateStatus(Game game) {
            if (game.IsWon) {
                return GameStatus.Won;
            }
            else if (game.Attempts.Count == 6) {
                return GameStatus.Finished;
            }
            else {
                return GameStatus.InProgress;
            }
        }
        public async Task<GameDto> MakeAttempt(int gameId, string attempt, string userId) {
            attempt = attempt.ToUpper();
            var game = await _context.Games
                .Include(g => g.Attempts)
                .Include(g => g.Word)
                .Where(g => g.Id == gameId && g.UserId == userId)
                .FirstOrDefaultAsync();

            if (game == null) {
                throw new Exception("Game not found for this user.");
            }

            var gameStatus = CalculateStatus(game);

            if (gameStatus != GameStatus.InProgress) {
                throw new GameAlreadyFinishedException("Game already finished.");
            }

            var newAttempt = new GameAttempt() {
                Game = game,
                GameId = game.Id,
                AttemptedWord = attempt,
                AttemptedAt = DateTime.Now
            };

            await _context.AddAsync(newAttempt);

            if (game.Word?.Text.ToUpper() == attempt) {
                game.IsWon = true;
            }

            await _context.SaveChangesAsync();

            return new GameDto() {
                Id = game.Id,
                Attempts = game.Attempts.Select(a => new AttemptDto() {
                    Attempt = a.AttemptedWord,
                    LettersState = CalculateLetterState(a)
                }).ToList(),
                GameStaus = CalculateStatus(game),
            };
        }
    }
}
