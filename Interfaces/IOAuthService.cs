using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poppin.Models.Identity.OAuth;

namespace Poppin.Interfaces
{
				/// <summary>
				/// https://www.youtube.com/watch?v=I2PChWTwmM8
				/// </summary>
				public interface IOAuthService
				{
								Task<IAccessTokenResult> GetAccessTokenAsync(string authCode);
								Task<IAccessTokenResult> GetAccessTokenAsync(string authCode, string redirectUri);
								Task<ITokenValidationResult> ValidateAccessTokenAsync(string accessToken);
								Task<IUserInfoResult> GetUserInfoAsync(string accessToken);
				}
}
