using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Auth;
using Medtrix_PmTool_V2_Api.Interfaces.Auth;
using Medtrix_PmTool_V2_Api.Services.Auth;
using Medtrix_PmTool_V2_Api.Services.Email;
using Medtrix_PmTool_V2_Api.Utills.ForgotPasswordHelper;

namespace Medtrix_PmTool_V2_Api.Repositories.Auth
{
    public class FBPasswordRepository : IForgotPassword
    {
        private readonly DbContext.DbContext _db;
        private readonly EmailService _email;

        public FBPasswordRepository(DbContext.DbContext db, EmailService email)
        {
            _db = db;
            _email = email;
        }

        public async Task<ForgotPasswordResponse> GenerateOtpAsync(ForgotPasswordRequest request)
        {
            using var conn=_db.CreateConnection();

            var response = new ForgotPasswordResponse();

            // Check if the email exists in the database
            var user =await conn.QueryFirstOrDefaultAsync<ForgotPasswordRequest>(
                "PMTool_v2.sp_check_email_exists", new { Email = request.Email }, commandType: System.Data.CommandType.StoredProcedure);

            if (user == null)
            {
              
                return null;
            }

            // Generate OTP
            response.Email = user.Email;
            response.Otp = FPHelper.GenerateOtp();

            // Mark previous OTPs as used
            await conn.ExecuteAsync(
                @"UPDATE [PMTool_v2].[Tbl_ForgotPassword_OTP]
      SET Used = 1
      WHERE Email = @Email
      AND Used = 0",
                new { Email = request.Email });

            // Save new OTP
            await conn.ExecuteAsync(
                @"INSERT INTO [PMTool_v2].[Tbl_ForgotPassword_OTP]
      (Email, OTP, Used, Expiry, CreatedOn)
      VALUES
      (@Email, @OTP, 0, DATEADD(MINUTE,10,GETDATE()), GETDATE())",
                new
                {
                    Email = request.Email,
                    OTP = response.Otp
                });

            var html = $@"
            <p>Hello,</p>

             <p>Your OTP for resetting your password is:</p>

             <h2>{response.Otp}</h2>

             <p>This OTP is valid for <b>10 minutes</b>.</p>

             <p>If you did not request a password reset, please ignore this email.</p>";

            await _email.SendAsync(
                request.Email,
                "",
                "Password Reset OTP",
                html);

            return response;
        }

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            using var conn = _db.CreateConnection();

            var hashedPassword = TokenService.HashPassword(request.NewPassword);

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "[PMTool_v2].[New_v2_sp_reset_forgot_password]",
                new
                {
                    Email = request.Email,
                    OTP = request.Otp,
                    NewPassword = hashedPassword
                },
                commandType: System.Data.CommandType.StoredProcedure);

            return result != null && result.Status == 1;
        }

        public async Task<bool> VerifyOtpAsync(VerifyPasswordRequest request)
        {
            using var conn = _db.CreateConnection();

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "[PMTool_v2].[New_v2_sp_verify_forgot_password_otp]",
                new
                {
                    Email = request.Email,
                    OTP = request.Otp
                },
                commandType: System.Data.CommandType.StoredProcedure);

            return result != null;
        }
    }
}
