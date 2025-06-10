using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Restaurant_WebAPI.Config;

namespace Restaurant_WebAPI.Util
{
    public class JwtHelper
    {
        public static string GenerateJwtToken(string userId)
        {
            var key = Encoding.UTF8.GetBytes(JwtConfig.Secret);
            var claims = new[] {
            new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var descriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(15),
                Issuer = JwtConfig.Issuer,
                Audience = JwtConfig.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var handler = new JwtSecurityTokenHandler();
            var token = handler.CreateToken(descriptor);
            return handler.WriteToken(token);
        }
    }
}
