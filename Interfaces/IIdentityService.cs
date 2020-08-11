using Microsoft.AspNetCore.Mvc;
using Poppin.Models.Identity;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface IIdentityService
				{
								Task<AuthenticationResult> RegisterAsync(string email, string password);
								Task<AuthenticationResult> LoginAsync(string email, string password);
								Task<UserDataResult> GetUser(string identifier);
				}
}
