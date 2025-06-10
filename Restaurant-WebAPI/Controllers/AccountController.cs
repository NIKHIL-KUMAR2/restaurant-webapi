using System;
using System.Security.Claims;
using System.Web.Http;
using Restaurant_WebAPI.Exceptions;
using Restaurant_WebAPI.Models;
using Restaurant_WebAPI.Interfaces;
using Restaurant_WebAPI.Util;
using Serilog;

namespace Restaurant_WebAPI.Controllers
{
    [RoutePrefix(Constants.AccountRoutePrefix)]
    public class AccountController : ApiController
    {
        private readonly IAccountService accountService;

        public AccountController(IAccountService accountService)
        {
            this.accountService = accountService;
        }

        [HttpPost]
        [Route("")]
        public IHttpActionResult Signup([FromBody] SignUpRequest model)
        {
            Log.Information("[ACCOUNT_SIGNUP_ATTEMPT] Signup attempt received.");

            try
            {
                if (model == null)
                {
                    Log.Warning("[ACCOUNT_SIGNUP_FAILED] Model was null.");
                    return BadRequest("Account details are missing or invalid");
                }

                if (!ModelState.IsValid)
                {
                    Log.Warning("[ACCOUNT_SIGNUP_FAILED] Invalid model state.");
                    return BadRequest(ModelState);
                }

                bool result = accountService.AddNewAccount(model);
                if (result)
                {
                    Log.Information("[ACCOUNT_SIGNUP_SUCCESS] Account created successfully.");
                    return Ok("Account created successfully");
                }
                else
                {
                    Log.Warning("[ACCOUNT_SIGNUP_FAILED] Service returned false.");
                    return BadRequest("Account not created! Please check your input.");
                }
            }
            catch (DatabaseException ex)
            {
                Log.Error(ex, "[ACCOUNT_SIGNUP_DB_EXCEPTION] Database exception during signup.");
                return BadRequest(ex.Message);
            }
            catch (AccountRequestException ex)
            {
                Log.Error(ex, "[ACCOUNT_SIGNUP_VALIDATION_EXCEPTION] Validation error during signup.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ACCOUNT_SIGNUP_EXCEPTION] Unexpected error during signup.");
                return InternalServerError(new Exception("Something went wrong! Please try again"));
            }
        }

        [Authorize]
        [HttpPatch]
        [Route("")]
        public IHttpActionResult UpdateAccount(UpdateRequest request)
        {
            Log.Information("[ACCOUNT_UPDATE_ATTEMPT] UpdateAccount request received.");

            try
            {
                if (request == null)
                {
                    Log.Warning("[ACCOUNT_UPDATE_FAILED] Request model was null.");
                    return BadRequest("Account details are missing or invalid");
                }

                if (!ModelState.IsValid)
                {
                    Log.Warning("[ACCOUNT_UPDATE_FAILED] Invalid model state.");
                    return BadRequest(ModelState);
                }

                bool hasAtLeastOneValue =
                    !string.IsNullOrWhiteSpace(request.Email) ||
                    !string.IsNullOrWhiteSpace(request.FirstName) ||
                    !string.IsNullOrWhiteSpace(request.LastName) ||
                    !string.IsNullOrWhiteSpace(request.Password);

                if (!hasAtLeastOneValue)
                {
                    Log.Warning("[ACCOUNT_UPDATE_FAILED] No fields provided for update.");
                    return BadRequest("At least one field must be provided for update.");
                }

                var userIdClaim = (User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Log.Warning("[ACCOUNT_UPDATE_UNAUTHORIZED] User ID claim missing.");
                    return Unauthorized();
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    Log.Warning("[ACCOUNT_UPDATE_UNAUTHORIZED] Invalid user ID format.");
                    return Unauthorized();
                }

                bool result = accountService.UpdateAccount(request, userId);
                if (result)
                {
                    Log.Information("[ACCOUNT_UPDATE_SUCCESS] Account updated for userId {UserId}.", userId);
                    return Ok("Account updated successfully");
                }
                else
                {
                    Log.Warning("[ACCOUNT_UPDATE_FAILED] Update failed for userId {UserId}.", userId);
                    return BadRequest("Account not updated! Please check your input.");
                }
            }
            catch (DatabaseException ex)
            {
                Log.Error(ex, "[ACCOUNT_UPDATE_DB_EXCEPTION] Database error during update.");
                return BadRequest(ex.Message);
            }
            catch (AccountRequestException ex)
            {
                Log.Error(ex, "[ACCOUNT_UPDATE_VALIDATION_EXCEPTION] Validation error during update.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ACCOUNT_UPDATE_EXCEPTION] Unexpected error during update.");
                return InternalServerError(new Exception("Something went wrong! Please try again"));
            }
        }

        [Authorize]
        [HttpPost]
        [Route("deactivate")]
        public IHttpActionResult DeactivateAccount()
        {
            Log.Information("[ACCOUNT_DEACTIVATE_ATTEMPT] Deactivation request received.");

            try
            {
                var userIdClaim = (User.Identity as ClaimsIdentity)?.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    Log.Warning("[ACCOUNT_DEACTIVATE_UNAUTHORIZED] User ID claim missing.");
                    return Unauthorized();
                }

                if (!int.TryParse(userIdClaim.Value, out int userId))
                {
                    Log.Warning("[ACCOUNT_DEACTIVATE_UNAUTHORIZED] Invalid user ID format.");
                    return Unauthorized();
                }

                bool result = accountService.DeactivateAccount(userId);
                if (result)
                {
                    Log.Information("[ACCOUNT_DEACTIVATE_SUCCESS] Account deactivated for userId {UserId}.", userId);
                    return Ok("Account deactivated successfully");
                }
                else
                {
                    Log.Warning("[ACCOUNT_DEACTIVATE_NO_OP] Account already deactivated for userId {UserId}.", userId);
                    return BadRequest("Account was already deactivated. No action needed.");
                }
            }
            catch (AccountRequestException ex)
            {
                Log.Error(ex, "[ACCOUNT_DEACTIVATE_VALIDATION_EXCEPTION] Validation error during deactivation.");
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "[ACCOUNT_DEACTIVATE_EXCEPTION] Unexpected error during deactivation.");
                return InternalServerError(new Exception("Something went wrong! Please try again"));
            }
        }
    }
}
