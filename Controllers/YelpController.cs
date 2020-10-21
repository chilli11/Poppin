using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using Poppin.Models.Yelp;

namespace Poppin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class YelpController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IYelpService _yelpService;
        public YelpController(ILocationService locationService ,IYelpService yelpService)
        {
            _locationService = locationService;
            _yelpService = yelpService;
        }

        // GET: api/yelp/4kMBvIEWPxWkWKFN__8SxQ
        [HttpGet("{yelpId}")]
        public async Task<YelpBusiness> GetBusiness(string yelpId)
        {
            return await _yelpService.GetBusiness(yelpId);
        }

        // POST: api/yelp/businesses
        [HttpPost("businesses")]
        public async Task<YelpBusinessSearchResponse> GetBusinessesSearch(YelpBusinessSearchParams searchParams)
        {
            return await _yelpService.GetBusinessSearch(searchParams);
        }

        // POST: api/yelp/match
        [HttpPost("match")]
        public async Task<YelpBusinessSearchResponse> GetBusinessMatch(YelpBusinessMatchParams searchParams)
        {
            return await _yelpService.GetBusinessMatch(searchParams);
        }

        // POST: api/yelp/match
        [HttpGet("match/{locId}")]
        public async Task<YelpBusinessSearchResponse> GetBusinessMatch(string locId)
        {
            var loc = await _locationService.Get(locId);
            var searchParams = new YelpBusinessMatchParams(loc);
            return await _yelpService.GetBusinessMatch(searchParams);
        }

        [HttpGet("categories")]
        public async Task<IEnumerable<YelpCategory>> GetYelpCategories()
								{
            return await _yelpService.GetCategory();
								}
    }
}