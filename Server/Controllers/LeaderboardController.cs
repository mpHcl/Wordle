using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController(ILeaderboardService leaderboardService) : ControllerBase {
        private readonly ILeaderboardService _leaderboardService = leaderboardService 
            ?? throw new ArgumentNullException(nameof(leaderboardService));

        // GET api/leaderboard?page=1&pageSize=10
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLeaderboard([FromQuery] int page = 1, [FromQuery] int pageSize = 10) {
            return Ok(await _leaderboardService.GetLeaderboard(page, pageSize));
        }
    }
}
