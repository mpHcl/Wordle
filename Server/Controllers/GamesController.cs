using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Exceptions;
using Server.Services;
using Server.Services.Interfaces;
using System.Security.Claims;

namespace Server.Controllers {
    /// <summary>
    /// Provides endpoints for managing Wordle games, daily challenges, and attempts.
    /// </summary>
    [ApiController]
    [Route("/api/[controller]")]
    public class GamesController(IDailyChallengeService dailyChallengeService, IWordleGameService wordleGameService) : ControllerBase {
        private readonly IDailyChallengeService _dailyChallengeService = dailyChallengeService 
            ?? throw new ArgumentNullException(nameof(dailyChallengeService));
        private readonly IWordleGameService _wordleGameService = wordleGameService
            ?? throw new ArgumentNullException(nameof(wordleGameService));

        /// <summary>
        /// Retrieves or creates today's daily challenge for the authenticated user.
        /// </summary>
        /// <remarks>
        /// <b>GET</b> api/games/daily-challenge
        ///
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – Daily challenge game.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>The daily challenge game.</returns>
        [Authorize]
        [HttpGet("daily-challenge")]
        public async Task<IActionResult> GetDailyChallenge() {
            var todaysChallange = await _dailyChallengeService.GetToday();
            todaysChallange ??= await _dailyChallengeService.CreateToday();

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var gameDto = await _wordleGameService.GetOrCreateGameForDailyChallenge(todaysChallange.ChallengeId, userId);

            return Ok(gameDto);
        }

        /// <summary>
        /// Creates a new game for the authenticated user.
        /// </summary>
        /// <remarks>
        /// <b>POST</b> api/games
        ///
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>201 Created</b> – New game created.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>The created game.</returns>
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> GetNewGame() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var gameDto = await _wordleGameService.CreateNewGameAsync(userId);

            // return 201 Created with location to GET api/games/{gameId}
            return CreatedAtAction(nameof(GetGame), new { gameId = gameDto.Id }, gameDto);
        }

        /// <summary>
        /// Submits a word attempt for the specified game.
        /// </summary>
        /// <remarks>
        /// <b>POST</b> api/games/{gameId}/attempt
        ///
        /// <para><b>Possible return codes:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – Attempt processed.</description></item>
        /// <item><description><b>400 Bad Request</b> – Invalid word.</description></item>
        /// <item><description><b>409 Conflict</b> – Game already finished.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="gameId">The game ID.</param>
        /// <param name="attempt">The attempted word.</param>
        /// <returns>The updated game state.</returns>
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

        /// <summary>
        /// Retrieves paginated games for the authenticated user.
        /// </summary>
        /// <remarks>
        /// <b>GET</b> api/games?page=1&amp;pageSize=10
        ///
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – List of games.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>User games.</returns>
        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetGames([FromQuery] int page = 1, [FromQuery] int pageSize = 10) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            var skip = (page - 1) * pageSize;
            var games = await _wordleGameService.GetUserGamesAsync(userId, skip, pageSize);

            return Ok(games);
        }

        /// <summary>
        /// Retrieves a specific game by ID for the authenticated user.
        /// </summary>
        /// <remarks>
        /// <b>GET</b> api/games/{gameId}
        ///
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – Game found.</description></item>
        /// <item><description><b>404 Not Found</b> – Game does not exist.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="gameId">The game ID.</param>
        /// <returns>The game.</returns>
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
