using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity
{
				public class AuthenticationResult
				{
								public string Token { get; set; }
								public bool Success { get; set; }
								public IEnumerable<string> Errors { get; set; }
				}

				public class AuthorizationResult
				{

				}

				public class UserDataResult
				{
								public IdentityUser User { get; set; }
								public bool Success { get; set; }
								public IEnumerable<string> Errors { get; set; }
				}

				public class UserListResult
				{
								public IEnumerable<IdentityUser> Users { get; set; }
								public bool Success { get; set; }
								public IEnumerable<string> Errors { get; set; }
				}
}
