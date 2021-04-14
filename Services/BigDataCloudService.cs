using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Poppin.Configuration;
using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using Poppin.Models;
using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Poppin.Services
{
    public class BigDataCloudService : IBigDataCloudService
    {
        private string _apiUrl = "https://api.bigdatacloud.net/data/";
        private HttpClient _httpClient;
        private IBigDataCloudSettings _bdcSettings;
        private ILocationService _locationService;
        private ILogger<BestTimeService> _logger;

        public BigDataCloudService(
            HttpClient httpClient,
            IBigDataCloudSettings bdcSettings,
            ILocationService locationService,
            ILogger<BestTimeService> logger)
        {
            _httpClient = httpClient;
            _bdcSettings = bdcSettings;
            _logger = logger;
        }

        public async Task<PoppinLocation> GetTimeZoneInfo(PoppinLocation loc)
        {
            var request = new BigDataCloudTZRequest(loc.Address.Coordinates, _bdcSettings.ApiKey);
            try
            {
                loc.TimeZone = await RetrieveAndDeserialize<PoppinTimeZone>("timezone-by-location", request.AsStringDictionary(), HttpMethod.Get);
                return loc;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
                return loc;
            }
        }

        private async Task<T> RetrieveAndDeserialize<T>(string path, IDictionary<string, string> queryParams, HttpMethod method)
        {
            var endpoint = _apiUrl + path;
            endpoint = QueryHelpers.AddQueryString(endpoint, queryParams);
            var request = new HttpRequestMessage(method, endpoint);

            var response = await _httpClient.SendAsync(request);
            var jsonSerializerSettings = new JsonSerializerSettings();
            jsonSerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;

            try
            {
                var stream = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(stream, jsonSerializerSettings);
            }
            catch (Exception e)
            {
                throw e;
            }
        }
    }
}
