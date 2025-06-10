using System;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Models;
using Restaurant_WebAPI.Util;
using Restaurant_WebAPI.Enums;
using Restaurant_WebAPI.Services;

namespace Restaurant_WebAPI.Services
{
    public class AuthServiceAPI : IAuthServiceAPI
    {
        private readonly IUserRepository userRepository;
        private readonly ITokenRepository tokenRepository;

        public AuthServiceAPI(IUserRepository userRepo, ITokenRepository tokenrepo)
        {
            userRepository = userRepo;
            tokenRepository = tokenrepo;
        }

        public AuthResult Login(string email, string password)
        {
            var user = userRepository.GetUserByEmail(email);
            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) || user.Role != UserRole.Customer || !user.IsActive)
            {
                return new AuthResult { IsValid = false };
            }
            
            //Removing Old refresh token if exist
            tokenRepository.RemoveRefreshTokenByUserId(user.Id.ToString());

            var accessToken = JwtHelper.GenerateJwtToken(user.Id.ToString());
            var refreshToken = Guid.NewGuid().ToString("n");
            var refreshExpiry = DateTime.UtcNow.AddDays(7);

            tokenRepository.SaveRefreshToken(user.Id.ToString(), refreshToken, refreshExpiry);

            return new AuthResult
            {
                IsValid = true,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                ExpiresIn = 900
            };
        }

        public AuthResult Refresh(string refreshToken)
        {
            var storedToken = tokenRepository.GetRefreshToken(refreshToken);
            if (storedToken == null || storedToken.ExpiresUtc < DateTime.UtcNow)
                return new AuthResult { IsValid = false };
            var newAccessToken = JwtHelper.GenerateJwtToken(storedToken.Subject);
            return new AuthResult
            {
                IsValid = true,
                AccessToken = newAccessToken,
            };
        }

        public bool Logout(string refreshToken)
        {
            int result = tokenRepository.RemoveRefreshToken(refreshToken);
            if (result == 1)
            {
                return true;
            }
            else if (result == 0)
            {
                return false;
            }
            else
            {
                throw new Exception("An unexpected error has occured while removing token");
            }
        }
    }
}
