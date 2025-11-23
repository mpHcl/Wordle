using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Server.Database;
using Server.Models;
using Server.Services;
using Server.Services.Interfaces;
using Shared.Dtos.Auth;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace Server.Controllers {
    /// <summary>
    /// Provides authentication endpoints for user registration, login, and token refresh.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<WordleUser> userManager, WordleDbContext context, IConfiguration configuration) : ControllerBase {
        private readonly UserManager<WordleUser> _userManager = userManager
            ?? throw new ArgumentNullException(nameof(userManager));
        private readonly WordleDbContext _context = context
            ?? throw new ArgumentNullException(nameof(context));
        private readonly IConfiguration configuration = configuration
            ?? throw new ArgumentNullException(nameof(configuration));

        /// <summary>
        /// Registers a new user.
        /// </summary>
        /// <remarks>
        /// <b>POST</b> api/auth/register
        ///
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – User created.</description></item>
        /// <item><description><b>400 Bad Request</b> – Validation errors.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="dto">Registration data.</param>
        /// <returns>A status message.</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto) {
            var user = new WordleUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded) {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "User registered successfully" });
        }

        /// <summary>
        /// Logs in a user and returns a JWT token along with user settings.
        /// </summary>
        /// <remarks>
        /// <b>POST</b> api/auth/login
        ///
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – Token and settings.</description></item>
        /// <item><description><b>401 Unauthorized</b> – Invalid credentials.</description></item>
        /// </list>
        /// </remarks>
        /// <param name="dto">Login data.</param>
        /// <returns>JWT token and settings.</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto) {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password)) {
                return Unauthorized(new { Message = "Invalid email or password" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] 
                ?? throw new InvalidOperationException("JWT Key not found in configuration."));
            
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.UserName 
                    ?? throw new InvalidDataException($"User {user.Id}, does not have username.")),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                ]),
                Expires = DateTime.UtcNow.AddHours(48),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var wordleUser = await _context.Users
                .Include(u => u.Settings)
                .Where(u => u.Id == user.Id)
                .FirstAsync();

            var settings = wordleUser.Settings;
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(new LoginResponseDto {
                Token = tokenHandler.WriteToken(token),
                Settings = new() {
                    ShowHints = settings.ShowHints,
                    DarkMode = settings.DarkMode,
                    HardMode = settings.HardMode,
                    HighContrastMode = settings.HighContrastMode
                }
            });
        }

        /// <summary>
        /// Refreshes the user's authentication token if it is close to expiration.
        /// </summary>
        /// <remarks>
        /// <b>GET</b> api/auth/refresh
        ///
        /// <para><b>Returns:</b></para>
        /// <list type="bullet">
        /// <item><description><b>200 OK</b> – New token issued.</description></item>
        /// <item><description><b>400 Bad Request</b> – Token not near expiration.</description></item>
        /// <item><description><b>401 Unauthorized</b> – Invalid or missing token.</description></item>
        /// </list>
        /// </remarks>
        /// <returns>New JWT token.</returns>
        [HttpGet("refresh")]
        [Authorize]
        public async Task<IActionResult> RefreshToken() {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value!;
            var user = await _context.Users.FindAsync(userId);
            var expClaim = User.FindFirst(JwtRegisteredClaimNames.Exp)?.Value;
            if (expClaim != null && long.TryParse(expClaim, out long exp)) {
                var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp).UtcDateTime;
                if (expirationTime > DateTime.UtcNow.AddDays(1)) {
                    return BadRequest("Token not close enough to expiration.");
                }
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"]
                ?? throw new InvalidOperationException("JWT Key not found in configuration."));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.UserName
                    ?? throw new InvalidDataException($"User {user.Id}, does not have username.")),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                ]),
                Expires = DateTime.UtcNow.AddHours(48),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature
                )
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return Ok(tokenHandler.WriteToken(token));
        }
    }
}
