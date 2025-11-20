using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services.Interfaces;
using Shared.Dtos;
using System.Security.Claims;

namespace Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController(IWordleSettingsService wordleGameService) : ControllerBase
    {

        private readonly IWordleSettingsService _wordleGameService = wordleGameService;

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<WordleSettings>>> GetWordleSettings() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var settings = await _wordleGameService.GetSettingsForUser(userId);

            return Ok(settings);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<WordleSettings>> UpdateWordleSettings(SettingsDto settings) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            try {
                await _wordleGameService.UpdateSettingsForUser(userId, settings);
                return Ok();
            } catch (Exception ex) {
                return BadRequest(ex.Message);
            }
          
        }
    }
}
