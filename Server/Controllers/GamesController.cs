using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Exceptions;
using Server.Services;
using Server.Services.Interfaces;
using System.Security.Claims;

namespace Server.Controllers {
    [ApiController]
    [Route("/api/[controller]")]
    public class GamesController(IDailyChallengeService dailyChallengeService, IWordleGameService wordleGameService) : ControllerBase {
        private readonly IDailyChallengeService _dailyChallengeService = dailyChallengeService 
            ?? throw new ArgumentNullException(nameof(dailyChallengeService));
        private readonly IWordleGameService _wordleGameService = wordleGameService
            ?? throw new ArgumentNullException(nameof(wordleGameService));

        // GET api/games/daily-challenge
        [Authorize]
        [HttpGet("daily-challenge")]
        public async Task<IActionResult> GetDailyChallenge() {
            var todaysChallange = await _dailyChallengeService.GetToday();
            todaysChallange ??= await _dailyChallengeService.CreateToday();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var gameDto = await _wordleGameService.GetOrCreateGameForDailyChallenge(todaysChallange.ChallengeId, userId);

            return Ok(gameDto);
        }

        // POST api/games
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetNewGame() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var gameDto = await _wordleGameService.CreateNewGameAsync(userId);

            // return 201 Created with location to GET api/games/{gameId}
            return CreatedAtAction(nameof(GetGame), new { gameId = gameDto.Id }, gameDto);
        }

        // POST api/games/{gameId}/attempt
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
            catch (InvalidWordAttemptException ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        // GET api/games?page=1&pageSize=10
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetGames([FromQuery] int page = 1, [FromQuery] int pageSize = 10) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            var skip = (page - 1) * pageSize;
            var games = await _wordleGameService.GetUserGamesAsync(userId, skip, pageSize);

            return Ok(games);
        }

        // GET api/games/{gameId}
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
