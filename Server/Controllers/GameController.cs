using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Server.Exceptions;
using Server.Services;
using Shared.Dtos;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers {
    [ApiController]
    [Route("/api/[controller]")]
    public class GameController : ControllerBase {
        private readonly IDailyChallengeService _dailyChallengeService;
        private readonly IWordleGameService _wordleGameService;

        public GameController(IDailyChallengeService dailyChallengeService, IWordleGameService wordleGameService) {
            _dailyChallengeService = dailyChallengeService;
            _wordleGameService = wordleGameService;
        }

        [Authorize]
        [HttpGet("daily_challenge")]
        public async Task<IActionResult> GetDailyChallenge() {
            var todaysChallange = await _dailyChallengeService.GetToday();

            if (todaysChallange is null) {
                todaysChallange = await _dailyChallengeService.CreateToday();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            var gameDto = await _wordleGameService.GetOrCreateGameForDailyChallenge(todaysChallange.ChallengeId, userId);

            return Ok(gameDto);
        }

        [Authorize]
        [HttpGet("new_game")]
        public async Task<IActionResult> GetNewGame() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            return Ok(await _wordleGameService.CreateNewGame(userId));
        }

        [Authorize]
        [HttpPost("{gameId}/attempt")]
        public async Task<IActionResult> PostAttempt(int gameId, [FromBody] string attempt) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            try {
                return Ok(await _wordleGameService.MakeAttempt(gameId, attempt, userId));
            }
            catch (GameAlreadyFinishedException ex) {
                return Conflict(new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("games")]
        public async Task<IActionResult> GetGames([FromQuery] int page = 1, [FromQuery] int pageSize = 10) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            var skip = (page - 1) * pageSize;
            var games = await _wordleGameService.GetUserGamesAsync(userId, skip, pageSize);

            return Ok(games);
        }

        [Authorize]
        [HttpGet("{GameId}")]
        public async Task<IActionResult> GetGame(int gameId) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            var game = await _wordleGameService.GetGameByIdAsync(userId, gameId);

            if (game is null) {
                return NotFound();
            }

            return Ok(game);
        }
    }
}
