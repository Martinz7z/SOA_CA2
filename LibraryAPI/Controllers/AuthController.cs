using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace LibraryAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // Simple hardcoded validation for demo
            if (request.Username == "admin" && request.Password == "password123")
            {
                var token = GenerateToken(request.Username, "Admin");
                return Ok(new
                {
                    message = "Login successful",
                    token = token,
                    username = request.Username,
                    role = "Admin"
                });
            }

            return Unauthorized(new { message = "Invalid username or password" });
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            // Simple registration for demo
            var token = GenerateToken(request.Username, "User");
            return Ok(new
            {
                message = "Registration successful",
                token = token,
                username = request.Username,
                role = "User"
            });
        }

        [HttpGet("test")]
        [Microsoft.AspNetCore.Authorization.AllowAnonymous]
        public IActionResult Test()
        {
            return Ok(new { message = "Auth controller is working" });
        }

        private string GenerateToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("LibraryAPISecretKey12345678901234567890"); // 32 chars!

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
            new Claim(ClaimTypes.Name, username),
            new Claim(ClaimTypes.Role, role)
        }),
                Expires = DateTime.UtcNow.AddHours(2),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public class LoginRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public class RegisterRequest
        {
            public string Username { get; set; }
            public string Password { get; set; }
            public string Email { get; set; }
        }
    }
}