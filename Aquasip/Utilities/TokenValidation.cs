namespace Aquasip.Utilities
{
    public class TokenValidation
    {
        public static int VerifyEmailInMinutes = 30;
        public static int PasswordResetInMinutes = 30;
        public static string ParseTokenEmailVerify(string plaintText, string key)
        {
            var values = plaintText.Split('&');
            string emailValue = values[0].Split('=')[1];
            string minitValue = values[1].Split('=')[1];
            string expirValue = values[2].Split('=')[1];
            return key == "email" ? emailValue : key == "minit" ? minitValue : key == "expir" ? expirValue : string.Empty;
        }
    }
}
