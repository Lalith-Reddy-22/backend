using Dapper;
using Medtrix_PmTool_V2_Api.DTOs.Auth;
using Medtrix_PmTool_V2_Api.Interfaces.Auth;
using Medtrix_PmTool_V2_Api.Services.Auth;
using Medtrix_PmTool_V2_Api.Services.Email;
using Medtrix_PmTool_V2_Api.Utills.ForgotPasswordHelper;
using System.Data;

namespace Medtrix_PmTool_V2_Api.Repositories.Auth
{
    public class ForgotPasswordRepository : IForgotPassword
    {
        private readonly DbContext.DbContext _db;
        private readonly EmailService _email;

        // email -> (otp, expiry)
        public ForgotPasswordRepository(DbContext.DbContext db, EmailService email)
        {
            _db    = db;
            _email = email;
        }

        public async Task<ForgotPasswordResponse> GenerateOtpAsync(ForgotPasswordRequest request)
        {
            using var conn = _db.CreateConnection();

            var p = new DynamicParameters();
            p.Add("@Email",   request.Email, DbType.String);
            p.Add("@Message", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);

            await conn.ExecuteAsync(
                "PMTool_v2.sp_check_email_exists",
                p,
                commandType: CommandType.StoredProcedure
            );

            string msg = p.Get<string>("@Message") ?? string.Empty;

            if (msg != "Email found")
                return new ForgotPasswordResponse { Email = request.Email, Otp = string.Empty };

            var otp = FPHelper.GenerateOtp();
            await conn.ExecuteAsync(
                @"UPDATE [PMTool_v2].[Tbl_ForgotPassword_OTP]
      SET Used = 1
      WHERE Email = @Email
      AND Used = 0",
                new { Email = request.Email });

            await conn.ExecuteAsync(
                @"INSERT INTO [PMTool_v2].[Tbl_ForgotPassword_OTP]
      (Email, OTP, Used, Expiry, CreatedOn)
      VALUES
      (@Email, @OTP, 0, DATEADD(MINUTE,10,GETDATE()), GETDATE())",
                new
                {
                    Email = request.Email,
                    OTP = otp
                });
            var html = $@"
                <p>Hi,</p>
                <p>Your OTP for password reset is: <b style='font-size:20px'>{otp}</b></p>
                <p>This OTP is valid for <b>10 minutes</b>.</p>
                <p>If you did not request this, please ignore this email.</p>";

            await _email.SendAsync(request.Email, "", "Password Reset OTP", html);

            return new ForgotPasswordResponse { Email = request.Email, Otp = otp };
        }

        //public Task<bool> VerifyOtpAsync(VerifyPasswordRequest request)
        //{
        //    //if (!_otpStore.TryGetValue(request.Email, out var stored))
        //    //    return Task.FromResult(false);

        //    //if (stored.Expiry < DateTime.UtcNow)
        //    //{
        //    //    _otpStore.TryRemove(request.Email, out _);
        //    //    return Task.FromResult(false);
        //    //}

        //    //return Task.FromResult(stored.Otp == request.Otp);



        //}

        public async Task<bool> ResetPasswordAsync(ResetPasswordRequest request)
        {
            using var conn = _db.CreateConnection();

            var result = await conn.QueryFirstOrDefaultAsync<dynamic>(
                "[PMTool_v2].[New_v2_sp_reset_forgot_password]",
                new
                {
                    Email = request.Email,
                    OTP = request.Otp,
                    NewPassword = TokenService.HashPassword(request.NewPassword)
                },
                commandType: CommandType.StoredProcedure);

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
                commandType: CommandType.StoredProcedure);

            return result != null;
        }
    }
}
