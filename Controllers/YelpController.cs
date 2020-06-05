﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        [HttpGet("{yelp_id}")]
        public async Task<YelpBusiness> GetBusinesses(string yelp_id)
        {
            return await _yelpService.GetBusiness(yelp_id);
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
        [HttpGet("match/{locationId}")]
        public async Task<YelpBusinessSearchResponse> GetBusinessMatch(string locationId)
        {
            var loc = _locationService.Get(locationId);
            var searchParams = new YelpBusinessMatchParams(loc);
            return await _yelpService.GetBusinessMatch(searchParams);
        }
    }
}