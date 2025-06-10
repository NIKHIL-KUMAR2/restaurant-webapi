using System;
using Restaurant_WebAPI.Models;

namespace Restaurant_WebAPI.Interfaces
{
    public interface ITokenRepository
    {
        void SaveRefreshToken(string userId, string token, DateTime expiresUtc);
        RefreshToken GetRefreshToken(string token);
        int RemoveRefreshToken(string token);
        int RemoveRefreshTokenByUserId(string userId);
    }
}
