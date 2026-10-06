namespace Medtrix_PmTool_V2_Api.DTOs.Auth
{
    public class ForgotPasswordDto
    {
    }

    public class ForgotPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
    }

public class ForgotPasswordResponse
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;
    }
    public class VerifyPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Otp { get; set; } = string.Empty;

    }
    public class ResetPasswordRequest
    {
        public string Email { get; set; } = string.Empty;
        public string NewPassword { get; set; } = string.Empty;

        public string Otp { get; set; } = string.Empty;

    }


}
