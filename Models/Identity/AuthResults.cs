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
}
