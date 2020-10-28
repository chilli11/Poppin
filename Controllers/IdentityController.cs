using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Configuration;
using Poppin.Contracts;
using Poppin.Contracts.Requests;
using Poppin.Contracts.Responses;
using Poppin.Extensions;
using Poppin.Interfaces;
using Poppin.Models.Identity.OAuth;
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
        private readonly IIdentityService _identityService;
        private readonly IOAuthSettings _oAuthSettings;
        private readonly IOAuthHandler _oAuthHandler;
        private readonly string stateGuid = "8d97a48c-c825-47e0-862b-3103fbd0382d";

        public IdentityController(
            IIdentityService idService,
            IOAuthSettings oAuthSettings,
            IOAuthHandler oAuthHandler
        )
        {
            _identityService = idService;
            _oAuthSettings = oAuthSettings;
            _oAuthHandler = oAuthHandler;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"><see cref="UserRegistrationRequest" /></param>
        /// <returns>400 (<see cref="AuthFailedResponse"/>) or 200 (<see cref="AuthSuccessResponse"/>)</returns>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                    {
                        Errors = ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage))
                    }
                );
            }

            var registrationResult = await _identityService.RegisterAsync(request.Email, request.Password, request.Password2, GetIpAddress());

            if (!registrationResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = registrationResult.Errors
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = registrationResult.Token
            });
        }

        [HttpGet("confirm-email/{id}")]
        public async Task<IActionResult> ConfirmEmail(string id, string t)
								{
            if (id == null || t == null)
												{
                return BadRequest();
												}

            var userResult = await _identityService.GetUserById(id);
            if (!userResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = userResult.Errors
                });
            }

            var result = await _identityService.ConfirmEmailAsync(userResult.User, t);
            if (!result.Succeeded)
												{
                return BadRequest(result);
												}
            return Ok(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"><see cref="UserLoginRequest" /></param>
        /// <returns>400 (<see cref="AuthFailedResponse"/>) or 200 (<see cref="AuthSuccessResponse"/>, with RefreshToken)</returns>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage))
                });
            }

            var loginResult = await _identityService.LoginAsync(request.Email, request.Password, GetIpAddress());

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

        /// <summary>
        /// From https://code-maze.com/password-reset-aspnet-core-identity/
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
												if (!ModelState.IsValid)
												{
																return BadRequest(new AuthFailedResponse
																{
																				Errors = ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage))
																}
																);
            }

            var authResult = await _identityService.StartPasswordResetAsync(request.Email, GetIpAddress());

            if (!authResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = authResult.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = authResult.Token,
                RefreshToken = authResult.RefreshToken
            });
        }

        [HttpGet("reset-password/{id}")]
        public async Task<IActionResult> ResetPassword(string id, string t)
        {
            if (id == null || t == null)
            {
                return BadRequest();
            }

            var userResult = await _identityService.GetUserById(id);
            if (!userResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = userResult.Errors
                });
            }

            return Ok(new AuthSuccessResponse
            {
                Token = t
            });
        }

        [HttpPost("reset-password/{id}")]
        public async Task<IActionResult> ResetPassword(string id, [FromBody] ResetPasswordRequest request)
        {
            {
                if (id == null || request.Token == null)
                {
                    return BadRequest();
                }

                if (!_identityService.IsValidPassword(request.Password))
                {
                    return BadRequest(new AuthFailedResponse
                    {
                        Errors = new[] { "Password does not meet requirements." }
                    });
                }

                if (request.Password != request.Password2)
                {
                    return BadRequest(new AuthFailedResponse
                    {
                        Errors = new[] { "Passwords do not match." }
                    });
                }

                var userResult = await _identityService.GetUserById(id);
                if (!userResult.Success)
                {
                    return BadRequest(new AuthFailedResponse
                    {
                        Errors = userResult.Errors
                    });
                }

                var result = await _identityService.ResetPasswordAsync(userResult.User, request.Token, request.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(result);
                }
                return Ok(result);
            }
        }

        /// <summary>
        /// Refreshes user token through cookie
        /// </summary>
        /// <returns>401 (<see cref="AuthFailedResponse"/>) or 200 (<see cref="AuthSuccessResponse"/>, with RefreshToken)</returns>
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _identityService.RefreshToken(refreshToken, GetIpAddress());

            if (response == null)
            {
                return Unauthorized(new AuthFailedResponse
                {
                    Errors = new[] { "Invalid token" }
                });
            }

            SetTokenCookie(response.RefreshToken);

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

        [HttpGet("{id}/refresh-tokens")]
        public async Task<IActionResult> GetRefreshTokens(string id)
        {
            var userResult = await _identityService.GetUserById(id);
            if (userResult.User == null) return NotFound();

            return Ok(userResult.User.RefreshTokens);
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
            var parameters = new NameValueCollection();
            parameters["client_id"] = _oAuthSettings.Facebook.ClientId;
            parameters["redirect_uri"] = this.Url.Action(nameof(FacebookAuth), nameof(IdentityController), null, "https");
            parameters["state"] = stateGuid;

            var uri = new UriBuilder("https", "www.facebook.com");
            uri.Path = "v8.0/dialog/oauth";
            uri.Query = parameters.ToString();
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

        /// <summary>
        /// Gets user id claim from current token
        /// </summary>
        /// <returns>String</returns>
        private string GetUserId()
        {
            if (HttpContext.User.Claims.Any())
            {
                return HttpContext.User.Claims.Single(u => u.Type == "Id").Value;
            }
            return string.Empty;
        }

        // helper methods

        /// <summary>
        /// Sets cookie, with a 7 day expiration
        /// </summary>
        /// <param name="token"></param>
        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
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