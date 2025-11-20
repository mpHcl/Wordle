using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController(IAchievementService achievementService) : ControllerBase {
        private readonly IAchievementService _achievementService = achievementService;

        [HttpGet("achievement/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAchievement(int id) {
            return Ok(await _achievementService.GetAchievementDetails(id));
        }


        [HttpGet("achievements")]
        [Authorize]
        public async Task<IActionResult> GetAchievements() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            return Ok(await _achievementService.GetAchievementsListForUser(userId));
        }
    }
}
