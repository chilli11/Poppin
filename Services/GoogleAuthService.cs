using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.Identity.OAuth;
using System.Net.Http;
using System.Threading.Tasks;
using static Poppin.Models.Identity.OAuth.Google;

namespace Poppin.Services
{
				public class GoogleAuthService : IOAuthService
				{
								private readonly HttpClient _httpClient;
								private const string AccessTokenUrl = "";
								private const string TokenValidationUrl = "";
								private const string UserInfoUrl = "";

								private readonly OAuthSettings _oAuthSettings;

								public GoogleAuthService(OAuthSettings oAuthSettings, HttpClient httpClient)
								{
												_oAuthSettings = oAuthSettings;
												_httpClient = httpClient;
								}

								public async Task<IAccessTokenResult> GetAccessTokenAsync(string authCode)
								{
												return new AccessTokenResult();
								}
								public async Task<IAccessTokenResult> GetAccessTokenAsync(string authCode, string redirectUri)
								{
												return new AccessTokenResult();
								}

								public async Task<IValidationData> ValidateAccessTokenAsync(string accessToken)
								{
												return new GoogleValidationData();
								}

								public async Task<IUserInfoResult> GetUserInfoAsync(string accessToken)
								{
												return new UserInfoResult();
								}
				}
}
