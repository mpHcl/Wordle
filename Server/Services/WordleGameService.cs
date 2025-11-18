using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Exceptions;
using Server.Models;
using Shared.Dtos;
using System;

namespace Server.Services {
    public class WordleGameService(
        WordleDbContext context,
        IAchievementService achievementService,
        ILeaderboardService leaderboardService
    ) : IWordleGameService {

        private readonly WordleDbContext _context = context;
        private readonly IAchievementService _achievementService = achievementService;
        private readonly ILeaderboardService _lederboardService = leaderboardService;
        private static readonly Random _random = new();

        private static List<State> CalculateLetterState(GameAttempt attempt, bool hardMode) {
            var wordExpected = attempt.Game?.Word?.Text.ToUpper();
            var wordAttempted = attempt.AttemptedWord.ToUpper();

            if (wordExpected is null) {
                return [];
            }

            var result = new List<State>();
            for (int i = 0; i < wordAttempted.Length; i++) {
                if (wordAttempted[i] == wordExpected[i]) {
                    result.Add(State.LetterGuessedCorrectPlace);
                }
                else if (!hardMode && wordExpected.Contains(wordAttempted[i])) {
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

            var gameStatus = CalculateStatus(game);
            var result = new GameDto() {
                Id = game.Id,
                GameStatus = gameStatus,
                Attempts = [.. game.Attempts.Select(a => new AttemptDto {
                    Attempt = a.AttemptedWord,
                    LettersState = CalculateLetterState(a, game.HardMode)
                })],
                Hints = game.Hints,
                HardMode = game.HardMode,
                Word = gameStatus != GameStatus.InProgress ? game.Word?.Text : null
            };

            return result;
        }

        private async Task<GameDto> CreateChallengeGame(int challengeId, string userId) {
            var user = await _context.Users.Include(u => u.Settings).FirstAsync(u => u.Id == userId);
            var dailyChallenge = await _context.DailyChallenges.FirstAsync(dc => dc.Id == challengeId);
            var game = new Game {
                User = user,
                UserId = userId,
                DailyChallenge = dailyChallenge,
                DailyChallangeId = challengeId,
                WordId = dailyChallenge.WordId,
                HardMode = false,
                Hints = false
            };

            _context.Games.Add(game);
            _context.SaveChanges();

            var gameDto = new GameDto {
                Id = game.Id,
                GameStatus = CalculateStatus(game),
                Attempts = [],
                HardMode = game.HardMode,
                Hints = game.Hints
            };

            return gameDto;
        }

        public async Task<GameDto> CreateNewGame(string userId) {
            var user = await _context.Users.Include(u => u.Settings).FirstAsync(u => u.Id == userId);
            var hardMode = user.Settings.HardMode;
            var hints = user.Settings.ShowHints;
            var wordsIds = await _context.Words.Select(w => w.Id).ToListAsync();
            var randomWordId = wordsIds[_random.Next(0, wordsIds.Count)];
            var randomWord = await _context.Words.Include(w => w.Category).FirstAsync(w => w.Id == randomWordId);

            var game = new Game {
                User = user,
                UserId = userId,
                Word = randomWord,
                WordId = randomWord.Id,
                HardMode = hardMode,
                Hints = hints
            };

            _context.Games.Add(game);
            _context.SaveChanges();

            var gameDto = new GameDto {
                Id = game.Id,
                GameStatus = CalculateStatus(game),
                Attempts = [],
                HardMode = game.HardMode,
                Hints = game.Hints,
                Category = game.Hints ? randomWord.Category.Name : null
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
                .Include(g => g.Word!.Category)
                .Where(g => g.Id == gameId && g.UserId == userId)
                .FirstOrDefaultAsync() ?? throw new Exception("Game not found for this user.");

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

            var newAchievements = await _achievementService.UpdateAchievements(userId);
            gameStatus = CalculateStatus(game);

            if (gameStatus == GameStatus.Finished) {
                await _lederboardService.AddLostGame(userId);
            }

            if (gameStatus == GameStatus.Won) {
                await _lederboardService.AddWonGame(userId, game.HardMode, game.Hints);
            }

            return new GameDto() {
                Id = game.Id,
                Attempts = [.. game.Attempts.Select(a => new AttemptDto() {
                    Attempt = a.AttemptedWord,
                    LettersState = CalculateLetterState(a, game.HardMode)
                })],
                GameStatus = gameStatus,
                HardMode = game.HardMode,
                Hints = game.Hints,
                Word = gameStatus != GameStatus.InProgress ? game.Word?.Text : null,
                Category = game.Hints ? game.Word?.Category.Name : null,
                NewAchievements = [.. newAchievements]
            };
        }

        public async Task<IEnumerable<GameDto>> GetUserGamesAsync(string userId, int skip, int pageSize) {
            var games = await _context.Games
                 .Where(g => g.UserId == userId)
                 .Include(g => g.Attempts)
                 .Include(g => g.Word)
                 .Include(g => g.Word!.Category)
                 .OrderByDescending(g => g.Id)
                 .Skip(skip)
                 .Take(pageSize)
                 .AsNoTracking()
                 .ToListAsync();

            var gameDtos = games.Select(g => new GameDto {
                Id = g.Id,
                GameStatus = CalculateStatus(g),
                Attempts = [.. g.Attempts.Select(a => new AttemptDto {
                    Attempt = a.AttemptedWord,
                    LettersState = CalculateLetterState(a, g.HardMode)
                })],
                HardMode = g.HardMode,
                Hints = g.Hints,
                Word = CalculateStatus(g) != GameStatus.InProgress ? g.Word?.Text : null,
                Category = g.Hints ? g.Word?.Category.Name : null
            }).ToList();

            return gameDtos;
        }

        public async Task<GameDto?> GetGameByIdAsync(string userId, int gameId) {
            var game = await _context.Games.Where(g => g.UserId == userId && g.Id == gameId)
                .Include(g => g.Attempts)
                .Include(g => g.Word)
                .Include(g => g.Word!.Category)
                .AsNoTracking()
                .FirstAsync();

            var result = new GameDto {
                Id = game.Id,
                GameStatus = CalculateStatus(game),
                Attempts = [.. game.Attempts.Select(a => new AttemptDto {
                    Attempt = a.AttemptedWord,
                    LettersState = CalculateLetterState(a, game.HardMode)
                })],
                HardMode = game.HardMode,
                Hints = game.Hints,
                Word = CalculateStatus(game) != GameStatus.InProgress ? game.Word!.Text : null,
                Category = game.Hints ? game.Word?.Category.Name : null
            };

            return result;
        }
    }
}
