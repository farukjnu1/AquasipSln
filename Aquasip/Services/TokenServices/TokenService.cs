using Microsoft.AspNetCore.DataProtection;

namespace Aquasip.Services.TokenServices
{
    public class TokenService:ITokenService
    {
        private readonly IDataProtector _protector;

        public TokenService(IDataProtectionProvider provider)
        {
            _protector = provider.CreateProtector("MyTokens");
        }

        public string Encrypt(string text)
        {
            return _protector.Protect(text);
        }

        public string Decrypt(string token)
        {
            return _protector.Unprotect(token);
        }
    }
}
