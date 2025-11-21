using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Server.Exceptions;
using Server.Services.Interfaces;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AchievementsController(IAchievementService achievementService) : ControllerBase {
        private readonly IAchievementService _achievementService = achievementService 
            ?? throw new ArgumentNullException(nameof(achievementService));

        // GET api/achievements/{id}
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

        // GET api/achievements
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAchievements() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            return Ok(await _achievementService.GetAchievementsListForUser(userId));
        }
    }
}
