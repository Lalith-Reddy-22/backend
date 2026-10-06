using Medtrix_PmTool_V2_Api.DTOs.Auth;
using Medtrix_PmTool_V2_Api.Interfaces.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Medtrix_PmTool_V2_Api.Controllers.Auth
{
    [Route("api/v1/auth")]
    [ApiController]
    public class ForgotPasswordController : ControllerBase
    {
        private readonly IForgotPassword _repo;

        public ForgotPasswordController(IForgotPassword repo)
        {
            _repo = repo;
        }

        // POST /api/v1/auth/forgot-password
        [HttpPost("forgot-password")]
        public async Task<IActionResult> SendOtp([FromBody] ForgotPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email))
                return BadRequest(new { message = "Email is required." });

            var result = await _repo.GenerateOtpAsync(request);

            if (result == null || string.IsNullOrEmpty(result.Otp))
                return NotFound(new { message = "Email not found." });

            return Ok(new { message = "OTP sent to your email." });
        }

        // POST /api/v1/auth/verify-otp
        [HttpPost("verify-otp")]
        public async Task<IActionResult> VerifyOtp([FromBody] VerifyPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Otp))
                return BadRequest(new { message = "Email and OTP are required." });

            var valid = await _repo.VerifyOtpAsync(request);

            if (!valid)
                return BadRequest(new { message = "Invalid or expired OTP." });

            return Ok(new { message = "OTP verified." });
        }

        // POST /api/v1/auth/reset-password
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Email) ||
                string.IsNullOrWhiteSpace(request.Otp) ||
                string.IsNullOrWhiteSpace(request.NewPassword))
                return BadRequest(new { message = "Email, OTP and new password are required." });

            var success = await _repo.ResetPasswordAsync(request);

            if (!success)
                return BadRequest(new { message = "Invalid or expired OTP." });

            return Ok(new { message = "Password reset successfully." });
        }
    }
}
