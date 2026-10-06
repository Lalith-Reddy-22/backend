using Medtrix_PmTool_V2_Api.DTOs.Auth;
using Medtrix_PmTool_V2_Api.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Auth
{
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuth _repo;

        public AuthController(IAuth repo)
        {
            _repo = repo;
        }

        /// POST /api/v1/auth/login
        /// Body: { "emailId": "user@example.com", "password": "hello@123" }
        /// Returns JWT access token + refresh token on success
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.EmailId) || string.IsNullOrWhiteSpace(request.Password))
                return BadRequest("Email and password are required.");

            var result = await _repo.LoginAsync(request);

            if (result == null)
                return Unauthorized("Invalid credentials or access not permitted.");

            return Ok(result);
        }

        /// POST /api/v1/auth/refresh
        /// Body: { "refreshToken": "<token>" }
        /// Returns a new access token + new refresh token (old one is invalidated)
        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequestDto request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
                return BadRequest("Refresh token is required.");

            var result = await _repo.RefreshTokenAsync(request.RefreshToken);

            if (result == null)
                return Unauthorized("Refresh token is invalid or expired.");

            return Ok(result);
        }
    }
}
