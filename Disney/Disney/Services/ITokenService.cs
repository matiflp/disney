using Disney.Models.Auth;


namespace Disney.Services
{
    public interface ITokenService
    {
        string BuildToken(string key, string issuer, LoginModel user);
        bool IsTokenValid(string key, string issuer, string token);
    }
}
