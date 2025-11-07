using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Shared.Dtos;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers
{
    [ApiController]
    [Route("/api/[controller]")]
    public class DailyChallengeController : ControllerBase
    {
        private readonly IDailyChallengeService _dailyChallengeService;
        private readonly IWordleGameService _wordleGameService;

        public DailyChallengeController(IDailyChallengeService dailyChallengeService, IWordleGameService wordleGameService)
        {
            _dailyChallengeService = dailyChallengeService;
            _wordleGameService = wordleGameService;
        }
        
        [Authorize]
        [HttpGet("game")]
        public async Task<IActionResult> GetDailyChallenge()
        {
            var todaysChallange = await _dailyChallengeService.GetToday();
            
            if (todaysChallange is null)
            {
                todaysChallange = await _dailyChallengeService.CreateToday();
            }

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            var gameDto = await _wordleGameService.GetOrCreateGameForDailyChallenge(todaysChallange.ChallengeId, userId);

            return Ok(gameDto);
        }
    }
}
