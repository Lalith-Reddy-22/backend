using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Auth;
using Medtrix_PmTool_V2_Api.Interfaces.Auth;
using Medtrix_PmTool_V2_Api.Services.Auth;
using System.Collections.Concurrent;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Auth
{
    public class AuthRepository : IAuth
    {
        private readonly DbContext.DbContext _db;
        private readonly TokenService _tokenService;

   
        private static readonly ConcurrentDictionary<string, (int EmpId, DateTime Expiry)> _refreshTokens = new();

        public AuthRepository(DbContext.DbContext db, TokenService tokenService)
        {
            _db           = db;
            _tokenService = tokenService;
        }

       
        public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
        {
            using var conn = _db.CreateConnection();
            string pass = TokenService.HashPassword(request.Password);

            var employee = await conn.QueryFirstOrDefaultAsync<LoginRawDto>(
                "SP_LoginData",
                new { emailid = request.EmailId, password =  pass},
                commandType: CommandType.StoredProcedure
            );

            if (employee == null) return null;

            return BuildTokenResponse(employee);
        }

        public async Task<LoginResponseDto?> RefreshTokenAsync(string refreshToken)
        {
            // Check if this refresh token exists and hasn't expired
            if (!_refreshTokens.TryGetValue(refreshToken, out var stored) || stored.Expiry < DateTime.UtcNow)
            {
                _refreshTokens.TryRemove(refreshToken, out _); // clean up expired token
                return null;
            }

            // Fetch the employee details fresh from DB using the stored empId
            using var conn = _db.CreateConnection();
            var employee = await conn.QueryFirstOrDefaultAsync<LoginRawDto>(
                "SELECT EmpId, EmailId, EmpNumber, Employee_Name, Designation, User_Type, User_Password_Check FROM Tbl_Employee WHERE EmpId = @EmpId AND IsActive = 1",
                new { EmpId = stored.EmpId }
            );

            if (employee == null) return null;

            // Remove old refresh token (rotation — prevents reuse)
            _refreshTokens.TryRemove(refreshToken, out _);

            return BuildTokenResponse(employee);
        }

    
        /// Shared helper — generates access + refresh tokens and stores the refresh token.
       
        private LoginResponseDto BuildTokenResponse(LoginRawDto employee)
        {
            var (accessToken, expiry) = _tokenService.GenerateAccessToken(employee);
            var refreshToken          = _tokenService.GenerateRefreshToken();
            var refreshExpiry         = DateTime.UtcNow.AddDays(7); // refresh token valid for 7 days

            // Store refresh token in memory with its expiry
            _refreshTokens[refreshToken] = (employee.EmpId, refreshExpiry);

            return new LoginResponseDto
            {
                EmpId              = employee.EmpId,
                EmailId            = employee.EmailId,
                EmpNumber          = employee.EmpNumber,
                Employee_Name      = employee.Employee_Name,
                Designation        = employee.Designation,
                User_Type          = employee.User_Type,
                AccessToken        = accessToken,
                RefreshToken       = refreshToken,
                AccessTokenExpiry  = expiry
            };
        }
    }
}
