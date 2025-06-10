using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Services
{
    public interface IAuthServiceAPI
    {
        AuthResult Login(string email, string password);
        AuthResult Refresh(string refreshToken);
        bool Logout(string refreshToken);
    }
}
