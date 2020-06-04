using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Poppin.Interfaces;
using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Poppin.Services
{
				public class YelpService : IYelpService
				{
								private readonly HttpClient _httpClient;
								private readonly string _apiKey = "mn6I-cotcvPa3KzwOXFLQgxommmMZ-PTKBMsRuNhRIz0UF-ULb-9lxv5LlOseZkFhjdbyzk1nFkyumJALKw5O7mEgFKriyR-OWxd8Yh0Y7ze5xnV_b6ywfwtp-u-XnYx";
								private readonly string _clientId = "Bap_m5iTSk_GaXl3IoY-5A";
								private readonly string _apiEndpoint = "https://api.yelp.com/v3";



								public YelpService(HttpClient httpClient)
								{
												httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
												_httpClient = httpClient;
								}

								public async Task<List<YelpCategory>> GetCategory()
								{
												var endpoint = "/categories";
												var request = new HttpRequestMessage(HttpMethod.Get, _apiEndpoint + endpoint);
												var response = await _httpClient.SendAsync(request);
												List<YelpCategory> output = new List<YelpCategory>();

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																output = JsonConvert.DeserializeObject<List<YelpCategory>>(stream);
												}
												catch (Exception e)
												{
																throw e;
												}

												return output;
								}

								public async Task<YelpCategory> GetCategory(string alias)
								{
												var endpoint = "/categories/" + alias;
												var request = new HttpRequestMessage(HttpMethod.Get, _apiEndpoint + endpoint);
												var response = await _httpClient.SendAsync(request);
												YelpCategory output;

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																output = JsonConvert.DeserializeObject<YelpCategory>(stream);
												}
												catch (Exception e)
												{
																throw e;
												}

												return output;
								}

								public async Task<YelpBusiness> GetBusiness(string id)
								{
												var endpoint = "/businesses/" + id;
												var request = new HttpRequestMessage(HttpMethod.Get, _apiEndpoint + endpoint);
												var response = await _httpClient.SendAsync(request);
												YelpBusiness output;

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																output = JsonConvert.DeserializeObject<YelpBusiness>(stream);
												}
												catch (Exception e)
												{
																throw e;
												}

												return output;
								}

								public async Task<YelpBusinessSearchResponse> GetBusinessSearch(IYelpSearchParams searchParams)
								{

												var endpoint = QueryHelpers.AddQueryString("/businesses/search", searchParams.AsStringDictionary());
												var request = new HttpRequestMessage(HttpMethod.Get, _apiEndpoint + endpoint);

												var response = await _httpClient.SendAsync(request);
												YelpBusinessSearchResponse output;

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																output = JsonConvert.DeserializeObject<YelpBusinessSearchResponse>(stream);
												}
												catch (Exception e)
												{
																throw e;
												}

												output.SearchParams = searchParams;
												return output;
								}

								public async Task<YelpBusinessSearchResponse> GetBusinessMatch(IYelpSearchParams searchParams)
								{

												var endpoint = QueryHelpers.AddQueryString("/businesses/matches", searchParams.AsStringDictionary());
												var request = new HttpRequestMessage(HttpMethod.Get, _apiEndpoint + endpoint);

												var response = await _httpClient.SendAsync(request);
												YelpBusinessSearchResponse output = new YelpBusinessSearchResponse();

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																output = JsonConvert.DeserializeObject<YelpBusinessSearchResponse>(stream);
												}
												catch (Exception e)
												{
																throw e;
												}

												output.Total = output.Businesses.Count();
												output.SearchParams = searchParams;
												return output;
								}

				}
}
