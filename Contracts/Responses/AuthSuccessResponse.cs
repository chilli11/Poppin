using Microsoft.AspNetCore.Identity;
using Poppin.Models.Identity;
using Newtonsoft.Json;

namespace Poppin.Contracts.Responses
{
				public class AuthSuccessResponse
				{
								public string Token { get; set; }

								[JsonIgnore] // refresh token is returned in http only cookie
								public string RefreshToken { get; set; }
				}

				public class UserSuccessResponse
				{
								public User User { get; set; }
				}
}
