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

namespace Poppin.Services
{
				public class YelpService : IYelpService
				{
								private readonly HttpClient _httpClient;
								private readonly string _apiKey = "mn6I-cotcvPa3KzwOXFLQgxommmMZ-PTKBMsRuNhRIz0UF-ULb-9lxv5LlOseZkFhjdbyzk1nFkyumJALKw5O7mEgFKriyR-OWxd8Yh0Y7ze5xnV_b6ywfwtp-u-XnYx";
								//private readonly string _clientId = "Bap_m5iTSk_GaXl3IoY-5A";
								private readonly string _apiEndpoint = "https://api.yelp.com/v3";

								private List<YelpBusiness> _yelpBusinessCache = new List<YelpBusiness>();

								public YelpService(HttpClient httpClient)
								{
												httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _apiKey);
												_httpClient = httpClient;
								}

								public async Task<IEnumerable<YelpCategory>> GetCategory()
								{
												var endpoint = "/categories?locale=en_US";
												var request = new HttpRequestMessage(HttpMethod.Get, _apiEndpoint + endpoint);
												var response = await _httpClient.SendAsync(request);
												YelpCategoriesResponse output = new YelpCategoriesResponse();

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																output = JsonConvert.DeserializeObject<YelpCategoriesResponse>(stream);
												}
												catch (Exception e)
												{
																throw e;
												}

												return output.Categories;
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
												YelpBusiness output;

												try
												{
																output = _yelpBusinessCache.Find(yb => yb.Id == id);
																if (output == null)
																{
																				var endpoint = "/businesses/" + id;
																				var request = new HttpRequestMessage(HttpMethod.Get, _apiEndpoint + endpoint);
																				var response = await _httpClient.SendAsync(request);
																				var stream = await response.Content.ReadAsStringAsync();
																				output = JsonConvert.DeserializeObject<YelpBusiness>(stream);
																				_yelpBusinessCache.Add(output);
																}
																
												}
												catch (Exception e)
												{
																throw e;
												}

												return output;
								}

								public async Task<YelpBusinessSearchResponse> GetBusinessSearch(YelpBusinessSearchParams searchParams)
								{
												var paramDictionary = searchParams.AsStringDictionary();
												var endpoint = QueryHelpers.AddQueryString("/businesses/search", paramDictionary);
												var request = new HttpRequestMessage(HttpMethod.Get, _apiEndpoint + endpoint);

												var response = await _httpClient.SendAsync(request);
												YelpBusinessSearchResponse output;

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																output = JsonConvert.DeserializeObject<YelpBusinessSearchResponse>(stream);
																output.Businesses.Where(yb => !_yelpBusinessCache.Contains(yb)).ToList()
																				.ForEach(yb => _yelpBusinessCache.Add(yb));
												}
												catch (Exception e)
												{
																throw e;
												}

												output.SearchParams = searchParams;
												return output;
								}

								public async Task<YelpBusinessSearchResponse> GetBusinessMatch(YelpBusinessMatchParams searchParams)
								{

												var endpoint = QueryHelpers.AddQueryString("/businesses/matches", searchParams.AsStringDictionary());
												var request = new HttpRequestMessage(HttpMethod.Get, _apiEndpoint + endpoint);

												var response = await _httpClient.SendAsync(request);
												YelpBusinessSearchResponse output = new YelpBusinessSearchResponse();

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																output = JsonConvert.DeserializeObject<YelpBusinessSearchResponse>(stream);
																output.Businesses.Where(yb => !_yelpBusinessCache.Contains(yb)).ToList()
																				.ForEach(yb => _yelpBusinessCache.Add(yb));
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
