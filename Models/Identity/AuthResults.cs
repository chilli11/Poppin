using Newtonsoft.Json;
using System.Collections.Generic;

namespace Poppin.Models.Identity
{
				public class AuthenticationResult
				{
								public string Token { get; set; }

								[JsonIgnore] // refresh token is returned in http only cookie
								public string RefreshToken { get; set; }
								public bool Success { get; set; }
								public IEnumerable<string> Errors { get; set; }
				}

				public class AuthorizationResult
				{

				}

				public class UserDataResult
				{
								public User User { get; set; }
								public bool Success { get; set; }
								public IEnumerable<string> Errors { get; set; }
				}

				public class UserListResult
				{
								public IEnumerable<User> Users { get; set; }
								public bool Success { get; set; }
								public IEnumerable<string> Errors { get; set; }
				}
}
