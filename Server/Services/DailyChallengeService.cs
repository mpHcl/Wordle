using Microsoft.EntityFrameworkCore;
using Server.Database;
using Server.Models;
using Server.Services.Interfaces;
using Shared.Dtos;

namespace Server.Services {
    public class DailyChallengeService(WordleDbContext context) : IDailyChallengeService {
        private readonly WordleDbContext _context = context 
            ?? throw new ArgumentNullException(nameof(context));

        private static readonly Random _random = new();

        public async Task<DailyChallengeDto> CreateToday() {
            var today = DateTime.UtcNow.Date;

            // Prevent duplicates
            var existing = await _context.DailyChallenges
                .FirstOrDefaultAsync(dc => dc.Date == DateOnly.FromDateTime(today));
            if (existing != null)
                return new DailyChallengeDto { ChallengeId = existing.Id };

            var wordsIds = await _context.Words.Select(w => w.Id).ToListAsync();
            var randomWordId = wordsIds[_random.Next(0, wordsIds.Count)];
            var word = await _context.Words.FirstAsync(w => w.Id == randomWordId);

            var dailyChallenge = new DailyChallenge {
                Date = DateOnly.FromDateTime(today),
                WordId = word.Id,
                Word = word
            };

            _context.DailyChallenges.Add(dailyChallenge);
            await _context.SaveChangesAsync();

            return new DailyChallengeDto { ChallengeId = dailyChallenge.Id };
        }

        public async Task<DailyChallengeDto?> GetToday() {
            var today = DateTime.UtcNow.Date;
            var dailyChallenge = await _context.DailyChallenges.FirstOrDefaultAsync(
                dc => dc.Date == DateOnly.FromDateTime(today)
            );

            if (dailyChallenge is null)
                return null;

            return new DailyChallengeDto { ChallengeId = dailyChallenge.Id };
        }
    }
}
