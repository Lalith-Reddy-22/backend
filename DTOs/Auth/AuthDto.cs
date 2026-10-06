namespace Medtrix_PmTool_V2_Api.DTOs.Auth
{
    public class LoginRequestDto
    {
        public string EmailId { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponseDto
    {
        public int EmpId { get; set; }
        public string EmailId { get; set; } = string.Empty;
        public string EmpNumber { get; set; } = string.Empty;
        public string Employee_Name { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int User_Type { get; set; }
        public string AccessToken { get; set; } = string.Empty;
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime AccessTokenExpiry { get; set; }
    }

    public class RefreshTokenRequestDto
    {
        public string RefreshToken { get; set; } = string.Empty;
    }

    
    public class LoginRawDto
    {
        public int EmpId { get; set; }
        public string EmailId { get; set; } = string.Empty;
        public string EmpNumber { get; set; } = string.Empty;
        public string Employee_Name { get; set; } = string.Empty;
        public string Designation { get; set; } = string.Empty;
        public int User_Type { get; set; }
        public string? User_Password_Check { get; set; }
        public string? PasswordHash { get; set; }
        public int Fin_Mod_Access { get; set; }
    }
}
