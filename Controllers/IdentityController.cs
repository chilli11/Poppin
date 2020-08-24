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

            var registrationResult = await _identityService.RegisterAsync(request.Email, request.Password, request.Password2);

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

            var loginResult = await _identityService.LoginAsync(request.Email, request.Password);

            if (!loginResult.Success)
            {
                return BadRequest(new AuthFailedResponse
                {
                    Errors = loginResult.Errors
                });
            }
            return Ok(new AuthSuccessResponse
            {
                Token = loginResult.Token
            });
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
    }
}