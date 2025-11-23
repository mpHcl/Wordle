using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Server.Exceptions;
using Server.Models;
using Server.Services;
using Server.Services.Interfaces;
using Shared.Dtos;
using System.Security.Claims;

namespace Server.Controllers
{
    /// <summary>
    /// Provides endpoints for retrieving and updating user Wordle settings.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class SettingsController(IWordleSettingsService wordleGameService) : ControllerBase
    {

        private readonly IWordleSettingsService _wordleGameService = wordleGameService 
            ?? throw new ArgumentNullException(nameof(wordleGameService));

        /// <summary>
        /// Retrieves the Wordle settings for the authenticated user.
        /// </summary>
        /// <remarks>
        /// <b>GET</b> api/settings
        ///
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – User settings.</description></item>
        /// <item><description><b>404 Not Found</b> – User settings not found.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>User settings.</returns>
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<WordleSettings>>> GetWordleSettings() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            
            try {
                var settings = await _wordleGameService.GetSettingsForUser(userId);
                return Ok(settings);
            }
            catch (ObjectNotFoundException ex) {
                return NotFound(ex.Message);
            }
        }

        /// <summary>
        /// Updates the Wordle settings for the authenticated user.
        /// </summary>
        /// <remarks>
        /// <b>POST</b> api/settings
        ///
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – Settings updated.</description></item>
        /// <item><description><b>404 Not Found</b> – User not found.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="settings">The updated settings.</param>
        /// <returns>Status result.</returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<WordleSettings>> UpdateWordleSettings(SettingsDto settings) {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;

            try {
                await _wordleGameService.UpdateSettingsForUser(userId, settings);
                return Ok();
            } catch (ObjectNotFoundException ex) {
                return NotFound(ex.Message);
            }
        }
    }
}
