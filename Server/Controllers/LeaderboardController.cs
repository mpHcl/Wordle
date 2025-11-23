using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Services;
using Server.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers {
    /// <summary>
    /// Provides leaderboard retrieval endpoints.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class LeaderboardController(ILeaderboardService leaderboardService) : ControllerBase {
        private readonly ILeaderboardService _leaderboardService = leaderboardService 
            ?? throw new ArgumentNullException(nameof(leaderboardService));

        /// <summary>
        /// Retrieves leaderboard entries, optionally filtered by username.
        /// </summary>
        /// <remarks>
        /// <b>GET</b> api/leaderboard?page=1&amp;pageSize=10&amp;filter=&quot;asd&quot;
        ///
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – leaderboard.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="page">Page number.</param>
        /// <param name="pageSize">Entries per page.</param>
        /// <param name="filter">Optional username filter.</param>
        /// <returns>Leaderboard entries.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetLeaderboard([FromQuery] int page = 1, [FromQuery] int pageSize = 10, [FromQuery] string filter = "") {
            if (filter.Equals("")) {
                return Ok(await _leaderboardService.GetLeaderboard(page, pageSize));
            }
            else {
                return Ok(await _leaderboardService.GetLeaderboard(page, pageSize, filter));
            }
        }
    }
}
