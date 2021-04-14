using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models;
using Poppin.Models.BusinessEntities;
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
		private IHERESettings _hereSettings;
		private string _apiUrl = "https://{0}.search.hereapi.com/v1/{0}";

		public HEREGeocoder(HttpClient httpClient, IHERESettings hereSettings)
		{
			_httpClient = httpClient;
			_hereSettings = hereSettings;
		}

		public async Task<Place> Lookup(IDictionary<string, string> queryParams) =>
			await RetrieveAndDeserialize<Place>("lookup", queryParams);

		public async Task<Geocode> Geocode(IDictionary<string, string> queryParams) {
			var result = await RetrieveAndDeserialize<Geocodes>("geocode", queryParams);
			return result.Items.First();
		}

		public async Task<IEnumerable<Place>> Discover(IDictionary<string, string> queryParams, GeoCoords at)
		{
			var result = await RetrieveAndDeserialize<Places>("discover", queryParams, at);
			return result.Items;
		}

		public async Task<IEnumerable<Place>> Browse(IDictionary<string, string> queryParams, GeoCoords at)
		{
			var result = await RetrieveAndDeserialize<Places>("browse", queryParams, at);
			return result.Items;
		}

		private async Task<T> RetrieveAndDeserialize<T>(string path, IDictionary<string, string> queryParams)
		{
			var endpoint = string.Format(_apiUrl, path);
			queryParams.Add("apiKey", _hereSettings.ApiKey);
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
