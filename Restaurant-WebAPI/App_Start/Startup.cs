using System;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Jwt;
using Owin;
using Restaurant_WebAPI.Config;

[assembly: OwinStartup(typeof(Restaurant_WebAPI.App_Start.Startup))]

namespace Restaurant_WebAPI.App_Start
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var key = Encoding.UTF8.GetBytes(JwtConfig.Secret);
            app.UseJwtBearerAuthentication(new JwtBearerAuthenticationOptions
            {
                AuthenticationMode = AuthenticationMode.Active,
                TokenValidationParameters = new TokenValidationParameters
                {
                    ValidIssuer = JwtConfig.Issuer,
                    ValidAudience = JwtConfig.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuerSigningKey = true,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                }
            });
        }
    }
}
