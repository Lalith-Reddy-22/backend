namespace Medtrix_PmTool_V2_Api.Utills.ForgotPasswordHelper
{
    public class FPHelper
    {

        // generate a random 6 digit otp

        public static string GenerateOtp()
        {
            return new Random().Next(100000, 999999).ToString();
        }




    }
}
