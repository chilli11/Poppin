using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
				public class PoppinUserRequest
				{
								public string UserId { get; set; }
								public string Username { get; set; }
								public string FirstName { get; set; }
								public string LastName { get; set; }
								public string Email { get; set; }
								public string ProfilePhoto { get; set; }
								public string AgeRange { get; set; }
								public string Gender { get; set; }
								public string[] Categories { get; set; }
				}
}
