using Poppin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface IIdentityService
				{
								Task<AuthenticationResult> RegisterAsync(string email, string password);
				}
}
