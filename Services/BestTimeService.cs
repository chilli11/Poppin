using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
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
        private string _apiUrl = "https://besttime.app/api/v1/";
        private HttpClient _httpClient;
        private IBestTimeSettings _btSettings;
        private ILocationService _locationService;
        private ILogger<BestTimeService> _logger;

        public BestTimeService(
            HttpClient httpClient,
            IBestTimeSettings btSettings,
            ILocationService locationService,
            ILogger<BestTimeService> logger)
        {
            _httpClient = httpClient;
            _btSettings = btSettings;
            _locationService = locationService;
            _logger = logger;
        }
        public BestTimeWeek GetRawWeek(string venueId)
        {
            return new BestTimeWeek();
        }
        public BestTimeWeek ForecastRawWeek(string venueId)
        {
            return new BestTimeWeek();
        }
        public async Task<BestTimeWeek> ForecastRawWeekAsync(PoppinLocation loc)
        {
            var request = new BestTimeRequest(loc, _btSettings.PrivateKey);
            loc.ForecastWeek = await RetrieveAndDeserialize<BestTimeWeek>("forecasts/week/raw2", request.AsStringDictionary(), HttpMethod.Post);
            loc.ForecastWeek.ForecastUpdatedOn = DateTime.Now;
            try
            {
                await _locationService.Update(loc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
            return loc.ForecastWeek;
        }
        public async Task StoreForecast(PoppinLocation loc)
        {
            var request = new BestTimeRequest(loc, _btSettings.PrivateKey);
            loc.ForecastWeek = await RetrieveAndDeserialize<BestTimeWeek>("forecasts/week/raw2", request.AsStringDictionary(), HttpMethod.Post);
            loc.ForecastWeek.ForecastUpdatedOn = DateTime.Now;
            try
            {
                await _locationService.Update(loc);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message, ex);
            }
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
