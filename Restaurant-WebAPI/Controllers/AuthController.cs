using System;
using System.Web.Http;
using Restaurant_WebAPI.Models;
using Restaurant_WebAPI.Services;
using Restaurant_WebAPI.Util;
using Serilog;

namespace Restaurant_WebAPI.Controllers
{
    [RoutePrefix(Constants.AuthRoutePrefix)]
    public class AuthController : ApiController
    {
        private readonly IAuthServiceAPI authService;

        public AuthController(IAuthServiceAPI authService)
        {
            this.authService = authService;
        }

        [HttpPost]
        [Route("login")]
        public IHttpActionResult Login([FromBody] LoginModel model)
        {
            if (model == null)
            {
                Log.Warning("[AUTH_LOGIN_FAILED] Login attempt failed: model was null.");
                return BadRequest("Login data is required.");
            }

            if (!ModelState.IsValid)
            {
                Log.Warning("[AUTH_LOGIN_FAILED] Invalid model state for login. Email: {Email}", model.Email);
                return BadRequest(ModelState);
            }

            try
            {
                Log.Information("[AUTH_LOGIN_ATTEMPT] Login attempt for email {Email}.", model.Email);

                var result = authService.Login(model.Email, model.Password);

                if (!result.IsValid)
                {
                    Log.Warning("[AUTH_LOGIN_FAILED] Invalid credentials for email {Email}.", model.Email);
                    return BadRequest("Invalid email or password.");
                }

                Log.Information("[AUTH_LOGIN_SUCCESS] Login successful for email {Email}.", model.Email);

                return Ok(new
                {
                    access_token = result.AccessToken,
                    refresh_token = result.RefreshToken,
                    token_type = "bearer",
                    expires_in = result.ExpiresIn
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[AUTH_LOGIN_EXCEPTION] Unexpected error during login for email {Email}.", model?.Email);
                return InternalServerError(new Exception("An unexpected error occurred. Please try again later."));
            }
        }

        [HttpPost]
        [Route("refresh")]
        public IHttpActionResult Refresh([FromBody] RefreshTokenModel model)
        {
            if (model == null)
            {
                Log.Warning("[AUTH_REFRESH_FAILED] Refresh token request failed: model was null.");
                return BadRequest("Refresh token data is required.");
            }

            if (!ModelState.IsValid)
            {
                Log.Warning("[AUTH_REFRESH_FAILED] Invalid model state during refresh token request.");
                return BadRequest(ModelState);
            }

            try
            {
                Log.Information("[AUTH_REFRESH_ATTEMPT] Refresh token attempt.");

                var result = authService.Refresh(model.RefreshToken);

                if (!result.IsValid)
                {
                    Log.Warning("[AUTH_REFRESH_FAILED] Invalid refresh token.");
                    return BadRequest("Invalid Token Provided");
                }

                Log.Information("[AUTH_REFRESH_SUCCESS] Refresh token successful.");

                return Ok(new
                {
                    access_token = result.AccessToken,
                    token_type = "bearer",
                    expires_in = result.ExpiresIn
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[AUTH_REFRESH_EXCEPTION] Unexpected error during token refresh.");
                return InternalServerError(new Exception("An unexpected error occurred. Please try again later."));
            }
        }

        [HttpPost]
        [Route("logout")]
        public IHttpActionResult Logout([FromBody] RefreshTokenModel model)
        {
            if (model == null)
            {
                Log.Warning("[AUTH_LOGOUT_FAILED] Logout request failed: model was null.");
                return BadRequest("Logout data is required.");
            }

            if (!ModelState.IsValid)
            {
                Log.Warning("[AUTH_LOGOUT_FAILED] Logout request failed: invalid model state.");
                return BadRequest(ModelState);
            }

            try
            {
                Log.Information("[AUTH_LOGOUT_ATTEMPT] Logout attempt.");

                bool result = authService.Logout(model.RefreshToken);

                if (result)
                {
                    Log.Information("[AUTH_LOGOUT_SUCCESS] Logout successful.");
                    return Ok("Logged out Successfully");
                }
                else
                {
                    Log.Warning("[AUTH_LOGOUT_FAILED] Logout failed: service returned false.");
                    return BadRequest("Failed while Logout");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[AUTH_LOGOUT_EXCEPTION] Unexpected error during logout.");
                return InternalServerError(new Exception("An unexpected error occurred. Please try again later."));
            }
        }
    }
}
