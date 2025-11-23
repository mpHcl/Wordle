using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Exceptions;
using Server.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers {
    /// <summary>
    /// Provides endpoints for retrieving achievements and achievement details.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController(IAchievementService achievementService) : ControllerBase {
        private readonly IAchievementService _achievementService = achievementService 
            ?? throw new ArgumentNullException(nameof(achievementService));


        /// <summary>
        /// Retrieves detailed information about a specific achievement.
        /// </summary>
        /// <remarks>
        /// <b>GET</b> api/achievements/{id}
        /// 
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – Achievement details.</description></item>
        /// <item><description><b>404 Not Found</b> – Achievement not found.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="id">The achievement ID.</param>
        /// <returns>The achievement details.</returns>
        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetAchievement(int id) {
            try {
                return Ok(await _achievementService.GetAchievementDetails(id));
            }
            catch (ObjectNotFoundException ex) {
                return NotFound(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Retrieves the list of achievements for the authenticated user, including completion status.
        /// </summary>
        /// <remarks>
        /// <b>GET</b> api/achievements
        /// 
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – User-specific achievements.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>A collection of achievements for the user.</returns>
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAchievements() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            return Ok(await _achievementService.GetAchievementsListForUser(userId));
        }
    }
}
