using Poppin.Interfaces;
using Poppin.Models.Identity.OAuth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static Poppin.Models.Identity.OAuth.Google;

namespace Poppin.Services
{
				public class GoogleAuthService : IOAuthService
				{
								private const string AccessTokenUrl = "";
								private const string TokenValidationUrl = "";
								private const string UserInfoUrl = "";

								public async Task<IAccessTokenResult> GetAccessTokenAsync(string authCode)
								{
												return new AccessTokenResult();
								}
								public async Task<IAccessTokenResult> GetAccessTokenAsync(string authCode, string redirectUri)
								{
												return new AccessTokenResult();
								}

								public async Task<ITokenValidationResult> ValidateAccessTokenAsync(string accessToken)
								{
												return new TokenValidationResult();
								}

								public async Task<IUserInfoResult> GetUserInfoAsync(string accessToken)
								{
												return new UserInfoResult();
								}
				}
}
