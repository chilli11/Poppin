using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models;
using Poppin.Models.Geocoding.HERE;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Poppin.Services
{
				public class HEREGeocoder : IHEREGeocoder
				{
								private HttpClient _httpClient;
								private HERESettings _hereSettings;
								private string _apiUrl = "https://{0}.search.hereapi.com/v1/{0}";

								public HEREGeocoder(HttpClient httpClient, IConfiguration configuration)
								{
												_httpClient = httpClient;
												configuration.Bind(nameof(_hereSettings), _hereSettings);

								}

								public Place Lookup(IDictionary<string, string> queryParams) =>
												RetrieveAndDeserialize<Place>("lookup", queryParams).Result;
								public Geocode Geocode(IDictionary<string, string> queryParams) =>
												RetrieveAndDeserialize<Geocodes>("geocode", queryParams).Result.Items.First();
								public IEnumerable<Place> Discover(IDictionary<string, string> queryParams, GeoCoords at) =>
												RetrieveAndDeserialize<Places>("discover", queryParams, at).Result.Items;
								public IEnumerable<Place> Browse(IDictionary<string, string> queryParams, GeoCoords at) =>
												RetrieveAndDeserialize<Places>("browse", queryParams, at).Result.Items;

								private async Task<T> RetrieveAndDeserialize<T>(string path, IDictionary<string, string> queryParams)
								{
												var endpoint = string.Format(_apiUrl, path);
												endpoint = QueryHelpers.AddQueryString(endpoint, queryParams);
												var request = new HttpRequestMessage(HttpMethod.Get, endpoint);

												var response = await _httpClient.SendAsync(request);

												try
												{
																var stream = await response.Content.ReadAsStringAsync();
																return JsonConvert.DeserializeObject<T>(stream);
												}
												catch (Exception e)
												{
																throw e;
												}
								}
								private Task<T> RetrieveAndDeserialize<T>(string path, IDictionary<string, string> queryParams, GeoCoords at)
								{
												queryParams.Add("at", at.Coordinates[1] + ", " + at.Coordinates[0]);
												return RetrieveAndDeserialize<T>(path, queryParams);
								}
				}
}
