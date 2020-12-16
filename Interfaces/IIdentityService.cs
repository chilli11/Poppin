using Microsoft.AspNetCore.Identity;
using Poppin.Contracts.Requests;
using Poppin.Models.Identity;
using Poppin.Models.Identity.OAuth;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
	public interface IIdentityService
	{
		Task<AuthenticationResult> RegisterAsync(string email, string password, string password2, string ipAddress);
		Task<IdentityResult> ConfirmEmailAsync(User user, string token);
		Task ResendConfirmationAsync(ForgotPasswordRequest request);
		Task<AuthenticationResult> LoginAsync(string email, string password, string ipAddress);
		Task<AuthenticationResult> OAuthLoginAsync(IUserInfoResult userInfo, string ipAddress);
		Task<AuthenticationResult> StartPasswordResetAsync(string email, string ipAddress);
		Task<IdentityResult> ResetPasswordAsync(User user, string token, string password);
		Task<UserDataResult> GetUserById(string identifier);
		Task<UserDataResult> GetUserByEmail(string Email);
		Task<UserListResult> GetUsersById(IEnumerable<string> ids);
		void Identify(User user, string category, string action);
		void Identify(string userId, string category, string action);

		//Task<AuthenticationResult> RefreshToken(string token, string ipAddress);
		Task<AuthenticationResult> RefreshToken(string token, string refreshToken, string ipAddress);
		Task<bool> RevokeToken(string token, string ipAddress);
		bool IsValidPassword(string password);
	}
}
