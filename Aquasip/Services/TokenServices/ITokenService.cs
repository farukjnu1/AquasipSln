namespace Aquasip.Services.TokenServices
{
    public interface ITokenService
    {
        string Encrypt(string text);
        string Decrypt(string token);   
    }
}
