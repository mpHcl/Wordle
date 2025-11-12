using System.Text;
using System.IdentityModel.Tokens.Jwt;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

using Microsoft.IdentityModel.Tokens;

using Server.Models;
using Shared.Dtos.Auth;
using System.Security.Claims;
using Server.Database;
using Microsoft.EntityFrameworkCore;


namespace Server.Controllers {
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(UserManager<WordleUser> userManager, WordleDbContext context, IConfiguration configuration) : ControllerBase {
        private readonly UserManager<WordleUser> _userManager = userManager;
        private readonly WordleDbContext _context = context;
        private readonly IConfiguration configuration = configuration;

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDto dto) {
            var user = new WordleUser { UserName = dto.UserName, Email = dto.Email };
            var result = await _userManager.CreateAsync(user, dto.Password);

            if (!result.Succeeded) {
                return BadRequest(result.Errors);
            }

            return Ok(new { Message = "User registered successfully" });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto) {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null || !await _userManager.CheckPasswordAsync(user, dto.Password)) {
                return Unauthorized(new { Message = "Invalid email or password" });
            }

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not found in configuration."));
            var tokenDescriptor = new SecurityTokenDescriptor {
                Subject = new ClaimsIdentity(
                [
                    new Claim(ClaimTypes.Name, user.UserName ?? throw new InvalidDataException($"User {user.Id}, does not have username.")),
                    new Claim(ClaimTypes.NameIdentifier, user.Id)
                ]),
                Expires = DateTime.UtcNow.AddHours(24),
                Issuer = configuration["Jwt:Issuer"],
                Audience = configuration["Jwt:Issuer"],
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var wordleUser = await _context.Users.Include(u => u.Settings).Where(u => u.Id == user.Id).FirstAsync();
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
    }
}
