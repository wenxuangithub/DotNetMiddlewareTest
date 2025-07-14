using Application.DTOs.Auth;
using Application.Interfaces;
using Infrastructure.Persistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using BCrypt.Net;
using Core.Entities;

namespace Api.Controllers
{
    [ApiController]
    [Route("auth")]
    public class AuthController : ControllerBase
    {
        private readonly ITokenService _tokenService;
        private readonly AppDbContext _db;

        public AuthController(ITokenService tokenService, AppDbContext db)
        {
            _tokenService = tokenService;
            _db = db;
        }

        [HttpPost("register")]
        public IActionResult Register([FromBody] RegisterRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();

            if (_db.Users.Any(u => u.Email == normalizedEmail))
                return Conflict(new { message = "Email already in use." });

            var hashed = BCrypt.Net.BCrypt.HashPassword(request.Password);

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = normalizedEmail,
                PasswordHash = hashed,
                Role = "User"
            };

            _db.Users.Add(user);
            _db.SaveChanges();

            return StatusCode(201, new { message = "User registered successfully." });
        }


        [HttpPost("login")]
        public IActionResult Login([FromBody] LoginRequest request)
        {
            var normalizedEmail = request.Email.Trim().ToLowerInvariant();
            var user = _db.Users.FirstOrDefault(u => u.Email == normalizedEmail);
            if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                return Unauthorized(new { message = "Invalid credentials." });

            var token = _tokenService.GenerateToken(
                userId: user.Id,
                email: user.Email,
                roles: new[] { user.Role }
            );

            return Ok(new LoginResponse { Token = token });
        }

        [Authorize]
        [HttpGet("check")]
        public IActionResult Check()
        {
            var user = HttpContext.User.Identity?.Name ?? "Unknown";
            return Ok($"Authenticated as: {user}");
        }

    }
}
