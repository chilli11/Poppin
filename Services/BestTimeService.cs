using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using Poppin.Configuration;
using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using Poppin.Models.BestTimeEntities;
using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Poppin.Services
{
    public class BestTimeService : IBestTimeService
    {
        private HttpClient _httpClient;
        private IBestTimeSettings _btSettings;
        private string _apiUrl = "https://besttime.app/api/v1/";

        public BestTimeService(HttpClient httpClient, IBestTimeSettings btSettings)
        {
            _httpClient = httpClient;
            _btSettings = btSettings;
        }
        public BestTimeWeek GetRawWeek(string venueId)
        {
            return new BestTimeWeek();
        }
        public BestTimeWeek ForecastRawWeek(string venueId)
        {
            return new BestTimeWeek();
        }
        public Task<BestTimeWeek> ForecastRawWeekAsync(PoppinLocation loc)
        {
            var request = new BestTimeRequest(loc, _btSettings.PrivateKey);
            return RetrieveAndDeserialize<BestTimeWeek>("forecasts/week/raw2", request.AsStringDictionary(), HttpMethod.Post);
        }

        private async Task<T> RetrieveAndDeserialize<T>(string path, IDictionary<string, string> queryParams, HttpMethod method)
        {
            var endpoint = _apiUrl + path;
            endpoint = QueryHelpers.AddQueryString(endpoint, queryParams);
            var request = new HttpRequestMessage(method, endpoint);

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
    }
}
