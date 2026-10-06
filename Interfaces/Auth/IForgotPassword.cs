using Medtrix_PmTool_V2_Api.DTOs.Auth;

namespace Medtrix_PmTool_V2_Api.Interfaces.Auth
{
    public interface IForgotPassword
    {

        Task<ForgotPasswordResponse> GenerateOtpAsync(ForgotPasswordRequest request); 
        Task<Boolean> VerifyOtpAsync(VerifyPasswordRequest request);

        Task<Boolean> ResetPasswordAsync(ResetPasswordRequest request);
    }
}
