using Application.DTOs.Auth;
using Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;

        public AuthController(ITokenService tokenService)
        {
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            // TEMPORARY: Hardcoded test user
            if (request.Email != "admin@example.com" || request.Password != "password")
                return Unauthorized(new { message = "Invalid credentials." });

            var token = _tokenService.GenerateToken(
                userId: Guid.NewGuid(),
                email: request.Email,
                roles: new[] { "Admin" }
            );

            return Ok(new LoginResponse { Token = token });
        }
    }
}
