using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Contracts;
using Poppin.Contracts.Requests;
using Poppin.Contracts.Responses;
using Poppin.Extensions;
using Poppin.Interfaces;

namespace Poppin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public IdentityController(IIdentityService idService, IHttpContextAccessor httpContextAccessor)
        {
            _identityService = idService;
            _httpContextAccessor = httpContextAccessor;
        }

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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = ModelState.Values.SelectMany(ms => ms.Errors.Select(e => e.ErrorMessage))
                }
                );
            }

            var loginResult = await _identityService.LoginAsync(request.Email, request.Password, GetIpAddress());

            if (!loginResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = loginResult.Errors
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = loginResult.Token,
                RefreshToken = loginResult.RefreshToken
            });
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];
            var response = await _identityService.RefreshToken(refreshToken, GetIpAddress());

            if (response == null)
            {
                var errors = new List<string>();
                errors.Add("Invalid token");
                return Unauthorized(new AuthFailedResponse
                {
                    Errors = errors
                });
            }

            SetTokenCookie(response.RefreshToken);

            return Ok(new AuthSuccessResponse
            {
                Token = response.Token,
                RefreshToken = response.RefreshToken
            });
        }

        [Authorize]
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken([FromBody] RevokeTokenRequest model)
        {
            // accept token from request body or cookie
            var token = model.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
            {
                var errors = new List<string>();
                errors.Add("Token is required");
                return BadRequest(new AuthFailedResponse
                {
                    Errors = errors
                });
            }

            var response = await _identityService.RevokeToken(token, GetIpAddress());

            if (!response)
            {
                var errors = new List<string>();
                errors.Add("Token not found");
                return NotFound(new AuthFailedResponse
                {
                    Errors = errors
                });
            }

            return Ok(new { message = "Token revoked" });
        }

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
        private string GetUserId()
        {
            if (_httpContextAccessor.HttpContext.User.Claims.Any())
            {
                return _httpContextAccessor.HttpContext.User.Claims.Single(u => u.Type == "Id").Value;
            }
            return string.Empty;
        }

        [HttpGet("{id}/refresh-tokens")]
        public async Task<IActionResult> GetRefreshTokens(string id)
        {
            var userResult = await _identityService.GetUserById(id);
            if (userResult.User == null) return NotFound();

            return Ok(userResult.User.RefreshTokens);
        }

        // helper methods

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(7)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string GetIpAddress()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];
            else
                return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
        }
    }
}