using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Exceptions;
using Server.Models;
using Server.Services.Interfaces;
using Shared.Dtos;
using System;

namespace Server.Services {
    public class WordleGameService(
        WordleDbContext context,
        IAchievementService achievementService,
        ILeaderboardService leaderboardService
    ) : IWordleGameService {

        private readonly WordleDbContext _context = context ?? throw new ArgumentNullException(nameof(context));
        private readonly IAchievementService _achievementService = achievementService ?? throw new ArgumentNullException(nameof(achievementService));
        private readonly ILeaderboardService _leaderboardService = leaderboardService ?? throw new ArgumentNullException(nameof(leaderboardService));
        private static readonly Random _random = new();

        private static GameStatus CalculateStatus(Game game) {
            if (game.IsWon)
                return GameStatus.Won;

            var attempts = game.Attempts.Count;
            return attempts >= 6 ? GameStatus.Finished : GameStatus.InProgress;
        }

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

        private static GameDto MapGameToDto(Game game) {
            var gameStatus = CalculateStatus(game);
            return new GameDto() {
                Id = game.Id,
                GameStatus = gameStatus,
                Attempts = [.. game.Attempts.Select(a => new AttemptDto {
                    Attempt = a.AttemptedWord,
                    LettersState = CalculateLetterState(a, game.HardMode)
                })],
                Hints = game.Hints,
                HardMode = game.HardMode,
                Category = game.Hints ? game.Word?.Category.Name : null,
                Word = gameStatus != GameStatus.InProgress ? game.Word?.Text : null
            };
        }

        public async Task<GameDto> GetOrCreateGameForDailyChallenge(int challengeId, string userId) {
            var gameDto = await GetChallengeGameAsync(challengeId, userId);
            return gameDto ?? await CreateChallengeGameAsync(challengeId, userId);
        }

        private async Task<GameDto?> GetChallengeGameAsync(int challengeId, string userId) {
            var game = await _context.Games
                .Where(game => game.DailyChallangeId == challengeId && game.UserId == userId)
                .Include(g => g.Attempts)
                .Include(g => g.Word)
                    .ThenInclude(w => w!.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            if (game is null) {
                return null;
            }

            
            return MapGameToDto(game);
        }

        private async Task<GameDto> CreateChallengeGameAsync(int challengeId, string userId) {
            var user = await _context.Users
                .Include(u => u.Settings)
                .FirstAsync(u => u.Id == userId)
                .ConfigureAwait(false);

            var dailyChallenge = await _context.DailyChallenges
                .FirstAsync(dc => dc.Id == challengeId)
                .ConfigureAwait(false);

            var game = new Game {
                User = user,
                UserId = userId,
                DailyChallenge = dailyChallenge,
                DailyChallangeId = challengeId,
                WordId = dailyChallenge.WordId,
                HardMode = false,
                Hints = false
            };

            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            return MapGameToDto(game);
        }

        public async Task<GameDto> CreateNewGameAsync(string userId) {
            var user = await _context.Users
                .Include(u => u.Settings)
                .FirstAsync(u => u.Id == userId)
                .ConfigureAwait(false);

            var hardMode = user.Settings.HardMode;
            var hints = user.Settings.ShowHints;

            var total = await _context.Words.CountAsync().ConfigureAwait(false);
            if (total == 0) throw new InvalidOperationException("No words available.");

            var skip = _random.Next(0, total);
            var randomWord = await _context.Words
                .Include(w => w.Category)
                .Skip(skip)
                .FirstAsync()
                .ConfigureAwait(false); 


            var game = new Game {
                User = user,
                UserId = userId,
                Word = randomWord,
                WordId = randomWord.Id,
                HardMode = hardMode,
                Hints = hints
            };

            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();


            return MapGameToDto(game);
        }

        public async Task<GameDto> MakeAttempt(int gameId, string attempt, string userId) {
            attempt = attempt.ToUpper();
            await ValidateAttemptAsync(attempt).ConfigureAwait(false);

            var game = await _context.Games
                .Include(g => g.Attempts)
                .Include(g => g.Word)
                    .ThenInclude(w => w!.Category)
                .Where(g => g.Id == gameId && g.UserId == userId)
                .FirstOrDefaultAsync()
                .ConfigureAwait(false)
                ?? throw new GameNotFoundException($"Game with id {gameId} not found for this user.");

            var statusBeforeAttempt = CalculateStatus(game);

            if (statusBeforeAttempt != GameStatus.InProgress) {
                throw new GameAlreadyFinishedException("Game already finished.");
            }

            var newAttempt = new GameAttempt() {
                Game = game,
                GameId = game.Id,
                AttemptedWord = attempt,
                AttemptedAt = DateTime.Now
            };

            await _context.AddAsync(newAttempt);

            if (string.Equals(game.Word?.Text, attempt, StringComparison.OrdinalIgnoreCase)) {
                game.IsWon = true;
            }

            await _context.SaveChangesAsync();

            var newAchievements = await _achievementService.UpdateAchievements(userId).ConfigureAwait(false);
            var statusAfterAttempt = CalculateStatus(game);

            await UpdateLeaderboardAsync(userId, game, statusAfterAttempt);

            var dto = MapGameToDto(game);
            dto.NewAchievements = [.. newAchievements];
            return dto;
        }

        private async Task ValidateAttemptAsync(string attempt) {
            var exists = await _context.WordsValidations
                .AsNoTracking()
                .AnyAsync(w => w.Text == attempt)
                .ConfigureAwait(false);

            if (!exists) {
                throw new InvalidWordAttemptException($"Word {attempt} is not a valid word.");
            }
        }

        private async Task UpdateLeaderboardAsync(string userId, Game game, GameStatus gameStatus) {
            if (gameStatus == GameStatus.Finished) {
                await _leaderboardService.AddLostGame(userId).ConfigureAwait(false);
            }

            if (gameStatus == GameStatus.Won) {
                await _leaderboardService.AddWonGame(userId, game.HardMode, game.Hints).ConfigureAwait(false);
            }
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
                 .ToListAsync()
                 .ConfigureAwait(false);

            return [.. games.Select(MapGameToDto)];
        }

        public async Task<GameDto?> GetGameByIdAsync(string userId, int gameId) {
            var game = await _context.Games.Where(g => g.UserId == userId && g.Id == gameId)
                .Include(g => g.Attempts)
                .Include(g => g.Word)
                    .ThenInclude(w => w!.Category)
                .AsNoTracking()
                .FirstOrDefaultAsync()
                .ConfigureAwait(false);

            return game is null ? null : MapGameToDto(game);
        }
    }
}
