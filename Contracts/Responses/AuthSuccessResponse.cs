using Poppin.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Responses
{
				public class AuthSuccessResponse
				{
								public string Token { get; set; }
				}

				public class UserSuccessResponse
				{
								public User User { get; set; }
				}
}
