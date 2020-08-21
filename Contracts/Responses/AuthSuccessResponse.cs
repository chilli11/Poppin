using Microsoft.AspNetCore.Identity;

namespace Poppin.Contracts.Responses
{
				public class AuthSuccessResponse
				{
								public string Token { get; set; }
				}

				public class UserSuccessResponse
				{
								public IdentityUser User { get; set; }
				}
}
