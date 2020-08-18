using Microsoft.AspNetCore.Mvc;
using Poppin.Models.Identity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface IIdentityService
				{
								Task<AuthenticationResult> RegisterAsync(string email, string password, string password2);
								Task<AuthenticationResult> LoginAsync(string email, string password);
								Task<UserDataResult> GetUserById(string identifier);
								Task<UserListResult> GetUsersById(IEnumerable<string> ids);
				}
}
