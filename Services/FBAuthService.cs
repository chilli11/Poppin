using Newtonsoft.Json;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.Identity.OAuth;
using System;
using System.Collections.Specialized;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using static Poppin.Models.Identity.OAuth.Facebook;

namespace Poppin.Services
{
				public class FBAuthService : IOAuthService
				{
								private readonly HttpClient _httpClient;
								private const string AccessTokenUrl = "https://graph.facebook.com/v8.0/oauth/access_token";
								private const string TokenValidationUrl = "https://graph.facebook.com/debug_token";
								private const string UserInfoUrl = "https://graph.facebook.com/me";

								private readonly IOAuthSettings _oAuthSettings;

								public FBAuthService(IOAuthSettings oAuthSettings, HttpClient httpClient)
								{
												_oAuthSettings = oAuthSettings;
												_httpClient = httpClient;
								}

								public async Task<IAccessTokenResult> GetAccessTokenAsync(string authCode)
								{
												var uri = new UriBuilder(AccessTokenUrl);
												var parameters = HttpUtility.ParseQueryString(string.Empty);

												parameters.Add("client_id", _oAuthSettings.Facebook.ClientId);

												return new AccessTokenResult();
								}

								public async Task<IAccessTokenResult> GetAccessTokenAsync(string authCode, string redirectUri)
								{
												var builder = new UriBuilder(AccessTokenUrl);
												var parameters = HttpUtility.ParseQueryString(string.Empty);

												parameters.Add("client_id", _oAuthSettings.Facebook.ClientId);
												parameters.Add("client_secret", _oAuthSettings.Facebook.Secret);
												parameters.Add("redirect_uri", redirectUri);
												parameters.Add("code", authCode);

												builder.Query = parameters.ToString();

												var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
												var response = await _httpClient.SendAsync(request);
												AccessTokenResult output;

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																output = JsonConvert.DeserializeObject<AccessTokenResult>(stream);
												}
												catch (Exception e)
												{
																throw e;
												}

												return output;
								}

								public async Task<IValidationData> ValidateAccessTokenAsync(string accessToken)
								{
												var builder = new UriBuilder(TokenValidationUrl);
												var parameters = HttpUtility.ParseQueryString(string.Empty);

												parameters.Add("access_token", _oAuthSettings.Facebook.ClientId + "|" + _oAuthSettings.Facebook.Secret);
												parameters.Add("input_token", accessToken);

												builder.Query = parameters.ToString();

												var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
												var response = await _httpClient.SendAsync(request);
												TokenValidationResult result;

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																result = JsonConvert.DeserializeObject<TokenValidationResult>(stream);
												}
												catch (Exception e)
												{
																throw e;
												}

												return result.Data;
								}

								public async Task<IUserInfoResult> GetUserInfoAsync(string accessToken)
								{
												var builder = new UriBuilder(UserInfoUrl);
												var parameters = HttpUtility.ParseQueryString(string.Empty);

												parameters.Add("fields", "first_name,last_name,email");
												parameters.Add("access_token", accessToken);

												builder.Query = parameters.ToString();

												var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
												var response = await _httpClient.SendAsync(request);
												UserInfoResult result;

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																result = JsonConvert.DeserializeObject<UserInfoResult>(stream);
												}
												catch (Exception e)
												{
																throw e;
												}

												return result;
								}
				}
}
