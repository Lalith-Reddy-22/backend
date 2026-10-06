using Medtrix_PmTool_V2_Api.DTOs.Auth;

namespace Medtrix_PmTool_V2_Api.Interfaces.Auth
{
    public interface IAuth
    {
        Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
        Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken);
    }
}
