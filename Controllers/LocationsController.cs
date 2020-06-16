using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using Poppin.Models;
using Poppin.Models.Yelp;
using Poppin.Services;

namespace Poppin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService;
        private readonly IYelpService _yelpService;
        public LocationsController(ILocationService locationService, IYelpService yelpService)
        {
            _locationService = locationService;
            _yelpService = yelpService;
        }

        // GET: api/Locations/5
        [HttpGet("{locationId}", Name = "Get")]
        public async Task<PoppinLocation> Get(string locationId)
        {
            var location = _locationService.Get(locationId);

            if (!string.IsNullOrEmpty(location.YelpId))
            {
                location.YelpDetails = await _yelpService.GetBusiness(location.YelpId);
            }

            return location;
        }

        // POST: api/Locations
        [HttpPost("yelp-search")]
        public async Task<PoppinSearchResponse> GetByYelpSearch(YelpBusinessSearchRequest searchParams)
        {
            var yelpSearchResponse = await _yelpService.GetBusinessSearch(searchParams);
            var locList = _locationService.GetByYelpList(yelpSearchResponse.Businesses);

            return new PoppinSearchResponse()
            {
                Total = locList.Count,
                Businesses = locList,
                Region = yelpSearchResponse.Region,
                SearchParams = yelpSearchResponse.SearchParams
            };
        }


        // POST: api/Locations
        [
            HttpPost,
            Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)
        ]
        public async Task<ActionResult<PoppinLocation>> Post(PoppinLocationRequest _location)
        {
            var location = new PoppinLocation(_location);
            var isExisting = _locationService.CheckExists(location);
            location.LastUpdate = DateTime.Now;


            if (isExisting == null)
            {
                await _locationService.Add(location);

                return CreatedAtAction("Post", location);
            }
            return isExisting;
        }

        // PUT: api/Locations/
        [
            HttpPut,
            Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)
        ]
        public void Put(PoppinLocationRequest _location)
        {
            var location = new PoppinLocation(_location);
            location.LastUpdate = DateTime.Now;
            _locationService.Update(location);
        }

        // PUT: api/Locations/5
        [HttpPut("{id}")]
        public void Put(string id, PoppinLocationRequest _location)
        {
            var location = new PoppinLocation(_location);
            location.LastUpdate = DateTime.Now;
            _locationService.Update(id, location);
        }

        // DELETE: api/Locations/5
        [HttpDelete("{id}")]
        public void Delete(string id)
        {
            _locationService.Delete(id);
        }
    }
}
