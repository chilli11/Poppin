using Microsoft.AspNetCore.Identity;
using Poppin.Models.Identity;
using Newtonsoft.Json;

namespace Poppin.Contracts.Responses
{
				public class AuthSuccessResponse
				{
								public string Token { get; set; }

								/// <summary>
								/// Only included in the cookie
								/// </summary>
								[JsonIgnore]
								public string RefreshToken { get; set; }
				}

				public class UserSuccessResponse
				{
								public User User { get; set; }
				}
}
