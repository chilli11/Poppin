using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Poppin.Models.Identity;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface IIdentityService
				{
								Task<AuthenticationResult> RegisterAsync(string email, string password, string password2, string ipAddress);
								Task<AuthenticationResult> LoginAsync(string email, string password, string ipAddress);
								Task<UserDataResult> GetUserById(string identifier);
								Task<UserDataResult> GetUserByEmail(string Email);
								Task<UserListResult> GetUsersById(IEnumerable<string> ids);
								void Identify(IdentityUser user, string category, string action);
								void Identify(string userId, string category, string action);

								Task<AuthenticationResult> RefreshToken(string token, string ipAddress);
								Task<bool> RevokeToken(string token, string ipAddress);
				}
}
