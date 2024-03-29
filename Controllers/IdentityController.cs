﻿using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Poppin.Configuration;
using Poppin.Contracts;
using Poppin.Contracts.Requests;
using Poppin.Contracts.Responses;
using Poppin.Extensions;
using Poppin.Interfaces;
using Poppin.Models.Identity.OAuth;
using Poppin.Models.Tracking;
using RTools_NTS.Util;

namespace Poppin.Controllers
{
    /// <summary>
    /// Handles all registration, login and token actions
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    //[ValidateAntiForgeryToken]
    public class IdentityController : PoppinBaseController
    {
        private readonly IOAuthSettings _oAuthSettings;
        private readonly IOAuthHandler _oAuthHandler;
        private readonly ISmtpService _smtpService;
        private readonly string stateGuid = "8d97a48c-c825-47e0-862b-3103fbd0382d";

        public IdentityController(
            IIdentityService idService,
            IOAuthSettings oAuthSettings,
            IOAuthHandler oAuthHandler,
            ISmtpService smtpService,
            ILogger<IdentityController> logger,
            ILogActionService logActionService,
            IUserService userService
        )
        {
            _identityService = idService;
            _oAuthSettings = oAuthSettings;
            _oAuthHandler = oAuthHandler;
            _smtpService = smtpService;
            _logger = logger;
            _logActionService = logActionService;
            _userService = userService;
        }

        /// <summary>
        /// Indicates whether the token is valid or not
        /// </summary>
        /// <returns>401 or 200</returns>
        [HttpGet("is-authenticated")]
        public IActionResult IsAuthenticated()
		{
            var userId = GetUserId();
            return Ok(new { authorized = !string.IsNullOrEmpty(userId) });
		}

        /// <param name="request"><see cref="UserRegistrationRequest" /></param>
        /// <returns>400 (<see cref="AuthFailedResponse"/>) or 200 (<see cref="AuthSuccessResponse"/>)</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Failed Registration: Invalid Model ({errors})", ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(new AuthFailedResponse
                    {
                        Errors = ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage))
                    }
                );
            }

            var registrationResult = await _identityService.RegisterAsync(request.Email, request.Password, request.Password2, GetIpAddress());

            if (!registrationResult.Success)
            {
                _logger.LogError("Failed Registration: {errors}", registrationResult.Errors);
                return BadRequest(new AuthFailedResponse
                {
                    Errors = registrationResult.Errors
                });
            }

            _logger.LogInformation("Successful Registration: {id}", request.Email);
            return Ok(new AuthSuccessResponse
            {
                Token = registrationResult.Token
            });
        }

        /// <summary>
        /// Confirms user's email address using token from email sent after registration
        /// or clicking on a resend confirmation link
        /// </summary>
        /// <param name="id"></param>
        /// <param name="t"></param>
        /// <returns>400 or <see cref="Microsoft.AspNetCore.Identity.IdentityResult"</returns>
        [HttpGet("confirm-email/{id}")]
        public async Task<IActionResult> ConfirmEmail(string id, string t)
								{
            if (id == null || t == null)
			{
                _logger.LogError("Confirm Email Failed: {errors}", new { Id = id, Token = t != null });
                return BadRequest();
			}

            var userResult = await _identityService.GetUserById(id);
            if (!userResult.Success)
            {
                _logger.LogError("Confirm Email: Find User Failed ({errors}", userResult.Errors);
                return BadRequest(new AuthFailedResponse
                {
                    Errors = userResult.Errors
                });
            }

            var result = await _identityService.ConfirmEmailAsync(userResult.User, t);
            if (!result.Succeeded)
            {
                _logger.LogError("Confirm Email Failed: {id}, {errors}", id, result.Errors);
                return BadRequest(result);
			}

            _userService.UpdateUser(userResult.User);
            _logger.LogInformation("Email Confirmed: {id}", id);
            _logActionService.LogUserAction(id, SegmentIOKeys.Actions.ConfirmEmail);
            return Ok(result);
        }

        /// <param name="request"><see cref="UserLoginRequest" /></param>
        /// <returns>400 (<see cref="AuthFailedResponse"/>) or 200 (<see cref="AuthSuccessResponse"/>, with RefreshToken)</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                _logger.LogError("Login Failed: Invalid Model ({errors})", ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage))
                });
            }

            var loginResult = await _identityService.LoginAsync(request.Email, request.Password, GetIpAddress());

            if (!loginResult.Success)
            {
                _logger.LogError("Login Failed for {id}: {errors}", request.Email, loginResult.Errors);
                return BadRequest(new AuthFailedResponse
                {
                    Errors = loginResult.Errors
                });
            }

            SetTokenCookie(loginResult.RefreshToken);
            _logger.LogInformation("Login Success: {id}", request.Email);
            return Ok(new AuthSuccessResponse
            {
                Token = loginResult.Token,
                RefreshToken = loginResult.RefreshToken
            });
        }

        /// <summary>
        /// Sends password reset email
        /// From https://code-maze.com/password-reset-aspnet-core-identity/
        /// </summary>
        /// <param name="request"><see cref="ForgotPasswordRequest"/></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
			if (!ModelState.IsValid)
            {
                _logger.LogError("Forgot Password Failed: Invalid Model ({errors})", ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage)));
                return BadRequest(new AuthFailedResponse
				{
					Errors = ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage))
				});
            }

            var authResult = await _identityService.StartPasswordResetAsync(request.Email, GetIpAddress());

            if (!authResult.Success)
            {
                _logger.LogError("Forgot Password Failed for {id}: {errors}", request.Email, authResult.Errors);
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResult.Errors
                });
            }

            _logger.LogInformation("Password Reset Email Sent: {id}", request.Email);
            _logActionService.LogUserAction(request.UserId, SegmentIOKeys.Actions.StartPasswordReset);
            return Ok(new AuthSuccessResponse
            {
                Token = authResult.Token,
                RefreshToken = authResult.RefreshToken
            });
        }

        /// <summary>
        /// Validates Password Reset Token
        /// </summary>
        /// <param name="id"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        [HttpGet("reset-password/{id}")]
        public async Task<IActionResult> ResetPassword(string id, string t)
        {
            if (id == null || t == null)
            {
                _logger.LogError("Reset Password Validation Failed: {errors}", new { Id = id, Token = t != null });
                return BadRequest();
            }

            var userResult = await _identityService.GetUserById(id);
            if (!userResult.Success)
            {
                _logger.LogError("Reset Password Validation Failed for {id}: {errors}", id, userResult.Errors);
                return BadRequest(new AuthFailedResponse
                {
                    Errors = userResult.Errors
                });
            }

            _logger.LogInformation("Reset Password Validated: {id}", id);
            return Ok(new AuthSuccessResponse
            {
                Token = t
            });
        }

        /// <summary>
        /// Resets password
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"><see cref="ResetPasswordRequest"/></param>
        /// <returns></returns>
        [HttpPost("reset-password/{id}")]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordRequest request)
        {
            {
                if (id == null || request.Token == null)
                {
                    _logger.LogError("Reset Password Failed: {errors}", new { Id = id, Token = request.Token != null });
                    return BadRequest();
                }

                if (!_identityService.IsValidPassword(request.Password))
                {
                    var errors = new[] { "Password does not meet requirements." };
                    _logger.LogError("Reset Password Failed for {id}: {errors}", id, errors);
                    return BadRequest(new AuthFailedResponse
                    {
                        Errors = errors
                    });
                }

                if (request.Password != request.Password2)
                {
                    var errors = new[] { "Passwords do not match." };
                    _logger.LogError("Reset Password Failed for {id}: {errors}", id, errors);
                    return BadRequest(new AuthFailedResponse
                    {
                        Errors = errors
                    });
                }

                var userResult = await _identityService.GetUserById(id);
                if (!userResult.Success)
                {
                    _logger.LogError("Reset Password Failed for {id}: {errors}", id, userResult.Errors);
                    return BadRequest(new AuthFailedResponse
                    {
                        Errors = userResult.Errors
                    });
                }

                var result = await _identityService.ResetPasswordAsync(userResult.User, request.Token, request.Password);
                if (!result.Succeeded)
                {
                    _logger.LogError("Reset Password Failed for {id}: {errors}", id, result.Errors);
                    return BadRequest(result.Errors);
                }

                _smtpService.SendPasswordConfirmationEmail(userResult.User);
                _userService.UpdateUser(userResult.User);

                _logger.LogInformation("Reset Password: {id}", id);
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.CompletePasswordReset);
                return Ok(result);
            }
        }

        /// <summary>
        /// Resends confirmation email to email address on file
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpPost("resend-confirmation")]
        public async Task<IActionResult> ResendConfirmation()
        {
            var userResult = await _identityService.GetUserById(GetUserId());
            var request = new ForgotPasswordRequest
            {
                Email = userResult.User.Email,
                UserId = userResult.User.Id.ToString()
            };

            try
            {

                await _identityService.ResendConfirmationAsync(request);
                _logger.LogInformation("Resent Confirmation Email: {id}", request.Email);
                _logActionService.LogUserAction(request.UserId, SegmentIOKeys.Actions.ResendConfirmationEmail);
                return Ok();
            }
            catch (Exception ex)
            {
                _logger.LogError("Resend Confirmation Failed for {id}: {exception}", request.Email, new { Exception = ex });
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// Refreshes user token through cookie
        /// </summary>
        /// <returns>401 (<see cref="AuthFailedResponse"/>) or 200 (<see cref="AuthSuccessResponse"/>, with RefreshToken)</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken(RefreshTokenRequest request)
        {
            var refreshToken = request.RefreshToken;
            var tokenType = string.IsNullOrEmpty(request.RefreshToken) ? "Cookie Token" : "Token";
            if (string.IsNullOrEmpty(refreshToken))
            {
                _logger.LogError(tokenType + " Refresh Failed: {errors}", new[] { "No token provided" });
                return Unauthorized(new AuthFailedResponse
                {
                    Errors = new[] { "Invalid token" }
                });
            }

            var response = await _identityService.RefreshToken(request.Token, request.RefreshToken, GetIpAddress());
            if (!response.Success)
            {
                _logger.LogError(tokenType + " Refresh Failed: {errors}", response.Errors);
                return Unauthorized(new AuthFailedResponse
                {
                    Errors = response.Errors
                });
            }

            SetTokenCookie(response.RefreshToken);

            _logger.LogInformation(tokenType + " Refreshed: {id}", GetUserId());
            return Ok(new AuthSuccessResponse
            {
                Token = response.Token,
                RefreshToken = response.RefreshToken
            });
        }

        /// <summary>
        /// Revokes included token
        /// </summary>
        /// <param name="model"></param>
        /// <returns>400/404 (<see cref="AuthFailedResponse"/>) or 200)</returns>
        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = new[] { "Token is required" }
                });
            }

            var response = await _identityService.RevokeToken(token, GetIpAddress());

            if (!response)
            {
                return NotFound(new AuthFailedResponse
                {
                    Errors = new[] { "Token not found" }
                });
            }

            return Ok(new { message = "Token revoked" });
        }

        /// <summary>
        /// Returns user Identity data; not useful for UI
        /// </summary>
        /// <returns>400 (<see cref="AuthFailedResponse"/>) or 200 (<see cref="UserSuccessResponse"/>)</returns>
        [Authorize]
		[HttpPost("me")]
        public async Task<IActionResult> GetUser()
        {
            var userResult = await _identityService.GetUserById(GetUserId());

            if (!userResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = userResult.Errors
                });
            }
            return Ok(new UserSuccessResponse
            {
                User = userResult.User
            });
        }

        /// <summary>
        /// Deprecated
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/refresh-tokens")]
        public async Task<IActionResult> GetRefreshTokens(string id)
        {
            var userResult = await _identityService.GetUserById(id);
            if (userResult.User == null) return NotFound();

            return Ok(new List<Models.Identity.RefreshToken>());
        }

        // ====================== OAUTH STUFF ====================== //

        // FACEBOOK
        /// <summary>
        /// Retrieve URL to FB login interface. Will redirect to /api/identity/facebook-auth with auth code
        /// </summary>
        /// <returns>{ fbLoginUri }</returns>
        [HttpGet("facebook-login-uri")]
        public IActionResult FacebookLoginUri()
								{
            var parameters = new NameValueCollection
            {
                ["client_id"] = _oAuthSettings.Facebook.ClientId,
                ["redirect_uri"] = this.Url.Action(nameof(FacebookAuth), nameof(IdentityController), null, "https"),
                ["state"] = stateGuid
            };

            UriBuilder uri = new UriBuilder("https", "www.facebook.com")
            {
                Path = "v8.0/dialog/oauth",
                Query = parameters.ToString()
            };
            return Ok(new { fbLoginUri = uri.ToString() });
								}

        /// <summary>
        /// Callback handler for FB login interface.
        /// Will log user in if successful
        /// </summary>
        /// <param name="code"></param>
        /// <returns cref="AuthSuccessResponse"></returns>
        [HttpGet("facebook-auth")]
        public async Task<IActionResult> FacebookAuth(string code)
        {
            var handler = _oAuthHandler.Services["Facebook"];
            try
            {
                var accessTokenResult = await handler.GetAccessTokenAsync(code, this.Url.Action(nameof(FacebookAuth), nameof(IdentityController), null, "https"));
                return await FacebookLogin(new OAuthLoginRequest { AccessToken = accessTokenResult.AccessToken });
            }
            catch (Exception ex)
            {
                _logger.LogError("Facebook Auth Failed: {exception}", ex);
                return BadRequest(new AuthFailedResponse
                {
                    Errors = new[] { ex.Message }
                });
            }
        }

        /// <summary>
        /// If access token is aqcuired elsewhere, or stored
        /// </summary>
        /// <param name="accessToken"></param>
        /// <returns cref="AuthSuccessResponse"></returns>
        [HttpPost("facebook-login")]
        public async Task<IActionResult> FacebookLogin(OAuthLoginRequest request)
        {
            var handler = _oAuthHandler.Services["Facebook"];
            var accessToken = request.AccessToken;
            var tokenValidation = await handler.ValidateAccessTokenAsync(accessToken);

            if (!tokenValidation.IsValid)
												{
                return BadRequest(new AuthFailedResponse
                {
                    Errors = new[] { "Invalid Facebook token" }
                });
												}

            var userInfo = await handler.GetUserInfoAsync(accessToken);
            if (userInfo.Email == null)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = new[] { "Something went wrong." }
                });
            }
            var loginResult = await _identityService.OAuthLoginAsync(userInfo, GetIpAddress());

            if (!loginResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = loginResult.Errors
                });
            }

            SetTokenCookie(loginResult.RefreshToken);
            return Ok(new AuthSuccessResponse
            {
                Token = loginResult.Token,
                RefreshToken = loginResult.RefreshToken
            });
        }

        [HttpGet("facebook-deauth")]
        public async Task<IActionResult> FacebookDeauth(string token)
        {
            return Ok();
        }

        [HttpGet("facebook-delete")]
        public async Task<IActionResult> FacebookDelete(string token)
        {
            return Ok();
        }

        // GOOGLE
        [HttpGet("google-auth")]
        public async Task<IActionResult> GoogleAuth(string token)
        {
            return Ok();
        }

        [HttpGet("google-deauth")]
        public async Task<IActionResult> GoogleDeauth(string token)
        {
            return Ok();
        }

        [HttpGet("google-delete")]
        public async Task<IActionResult> GoogleDelete(string token)
        {
            return Ok();
        }

        // helper methods

        /// <summary>
        /// Sets cookie, with a 120 day expiration
        /// </summary>
        /// <param name="token"></param>
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(120)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        /// <summary>
        /// Gets user IP Address for login/registration logs
        /// </summary>
        /// <returns>String</returns>
        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}