using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Contracts;
using Poppin.Contracts.Requests;
using Poppin.Contracts.Responses;
using Poppin.Interfaces;

namespace Poppin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class IdentityController : ControllerBase
    {
        private readonly IIdentityService _identityService;

        public IdentityController(IIdentityService idService)
        {
            _identityService = idService;
        }

        [HttpPost(ApiRoutes.Identity.Register)]
        public async Task<IActionResult> Register([FromBody] UserRegistrationRequest request)
        {
            var registrationResult = await _identityService.RegisterAsync(request.Email, request.Password);

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
    }
}