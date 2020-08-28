using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Contracts.Requests;
using Poppin.Contracts.Responses;
using Poppin.Extensions;
using Poppin.Interfaces;
using Poppin.Models;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Identity;
using Poppin.Models.Tracking;
using Poppin.Models.Yelp;
using Poppin.Services;

namespace Poppin.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LocationsController : ControllerBase
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        private readonly ILocationService _locationService;
        private readonly IYelpService _yelpService;
        private readonly IVendorService _vendorService;
        private readonly ILogActionService _logActionService;

        public LocationsController(
            IHttpContextAccessor httpContextAccessor,
            ILocationService locationService,
            IUserService userService,
            IYelpService yelpService,
            IVendorService vendorService,
            ILogActionService logActionService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _locationService = locationService;
            _yelpService = yelpService;
            _vendorService = vendorService;
            _logActionService = logActionService;
        }

        /// <summary>
        /// Gets location info based on ID passed as URL param.
        /// Includes <c>YelpDetails</c> if <c>YelpId</c> is available
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        // GET: api/Locations/5
        [HttpGet("{locationId}", Name = "Get")]
        public async Task<IActionResult> Get(string locationId)
        {
            var location = await _locationService.Get(locationId);

            if (location == null)
            {
                var errors = new List<string>();
                errors.Add("Location ID is invalid");
                return BadRequest(new GenericFailure
                {
                    Errors = errors
                });
            }
            if (!string.IsNullOrEmpty(location.YelpId))
            {
                location.YelpDetails = await _yelpService.GetBusiness(location.YelpId);
            }

            var action = new BasicLocationAction()
            {
                LocationId = location.Id
            };
            _logActionService.LogUserAction(GetUserId(), (int)ActionTypes.ViewLocation, action);

            return Ok(location);
        }

        /// <summary>
        /// Gets a list of Locations based on results of a Yelp search
        /// </summary>
        /// <param name="searchParams"></param>
        /// <returns></returns>
        // POST: api/Locations/yelp-search
        [HttpPost("yelp-search")]
        public async Task<IActionResult> GetByYelpSearch(YelpBusinessSearchParams searchParams)
        {
            try
            {
                var id = GetUserId();
                var yelpSearchResponse = await _yelpService.GetBusinessSearch(searchParams);
                var locList = new List<PoppinLocation>();
                if (yelpSearchResponse.Total > 0)
                {
                    locList = await _locationService.GetByYelpList(yelpSearchResponse.Businesses.Select(y => y.Id));
                    locList = locList.Where(l => !GetUserProfile(id).Hidden.Contains(l.Id)).ToList();
                    locList.ForEach(l => l.YelpDetails = yelpSearchResponse.Businesses.FirstOrDefault(yb => yb.Id == l.YelpId));
                }

                var action = new SearchAction()
                {
                    SearchTerm = searchParams.term,
                    SearchLocation = searchParams.location,
                    SearchCategories = searchParams.categories
                };
                _logActionService.LogUserAction(id, (int)ActionTypes.Search, action);

                return Ok(new PoppinSearchResponse()
                {
                    Total = locList.Count,
                    Businesses = locList,
                    Region = yelpSearchResponse.Region,
                    SearchParams = yelpSearchResponse.SearchParams
                });
            }
            catch(Exception e)
            {
                var errors = new List<string>();
                errors.Add(e.Message);
                return BadRequest(new GenericFailure
                {
                    Errors = errors
                });
            }
        }


        // POST: api/Locations
        [HttpPost]
        [Authorize]
        // [AuthorizeRoles()]
        public async Task<IActionResult> Post(PoppinLocationRequest _location)
        {
            if (GetUserRole() != RoleTypes.Admin)
												{
                return Unauthorized();
												}
            var location = new PoppinLocation(_location);
            var isExisting = await _locationService.CheckExists(location);
            location.LastUpdate = DateTime.Now;

            if (isExisting == null)
            {
                await _locationService.Add(location);
                var action = new BasicLocationAction()
                {
                    LocationId = location.Id
                };
                _logActionService.LogUserAction(GetUserId(), (int)ActionTypes.AddLocation, action);

                if (!string.IsNullOrEmpty(location.YelpId))
                {
                    location.YelpDetails = await _yelpService.GetBusiness(location.YelpId);
                }
                return CreatedAtAction("Post", location);
            }
            return Ok(isExisting);
        }

        // PUT: api/Locations/
        [HttpPut]
        [Authorize]
        // [AuthorizeRoles()]
        public async Task<IActionResult> Put(PoppinLocationRequest _location)
        {
            if (GetUserRole() != RoleTypes.Admin)
            {
                return Unauthorized();
            }
            var location = new PoppinLocation(_location);
            try
            {
                location.LastUpdate = DateTime.Now;
                await _locationService.Update(location);
                var action = new BasicLocationAction()
                {
                    LocationId = location.Id
                };
                _logActionService.LogUserAction(GetUserId(), (int)ActionTypes.UpdateLocation, action);

                if (!string.IsNullOrEmpty(location.YelpId))
                {
                    location.YelpDetails = await _yelpService.GetBusiness(location.YelpId);
                }
                return Ok(location);
            }
            catch(Exception e)
            {
                var errors = new List<string>();
                errors.Add(e.Message);
                return BadRequest(new GenericFailure
                {
                    Errors = errors
                });
            }
        }

        // PUT: api/Locations/5
        [HttpPut("{locationId}")]
        [Authorize]
        //[AuthorizeRoles()]
        public async Task<IActionResult> Put(string locationId, PoppinLocationRequest _location)
        {
            if (GetUserRole() != RoleTypes.Admin)
            {
                return Unauthorized();
            }
            var location = new PoppinLocation(_location);
            try
            {
                location.LastUpdate = DateTime.Now;
                await _locationService.Update(locationId, location);
                var action = new BasicLocationAction()
                {
                    LocationId = locationId
                };
                _logActionService.LogUserAction(GetUserId(), (int)ActionTypes.UpdateLocation, action);

                if (!string.IsNullOrEmpty(location.YelpId))
                {
                    location.YelpDetails = await _yelpService.GetBusiness(location.YelpId);
                }
                return Ok(location);
            }
            catch (Exception e)
            {
                var errors = new List<string>();
                errors.Add(e.Message);
                return BadRequest(new GenericFailure
                {
                    Errors = errors
                });
            }
        }

        // DELETE: api/Locations/5
        [HttpDelete("{locationId}")]
        [Authorize]
								//[AuthorizeRoles()]
								public IActionResult Delete(string locationId) {
            if (GetUserRole() != RoleTypes.Admin)
												{
                return Unauthorized();
            }
            _locationService.Delete(locationId);
            var action = new BasicLocationAction()
            {
                LocationId = locationId
            };
            _logActionService.LogUserAction(GetUserId(), (int)ActionTypes.DeleteLocation, action);
            return Ok();
        }

        // GET: api/Locations/incrementCrowd/5
        [HttpGet("incrementCrowd/{locationId}")]
        [Authorize]
        //[AuthorizeRoles(RoleTypes.Vendor, RoleTypes.Admin)]
        public async Task<IActionResult> IncrementCrowd(string locationId)
        {
            var location = await _locationService.Get(locationId);
            var errors = new List<string>();
            if (location == null)
            {
                errors.Add("Location ID is invalid");
                return BadRequest(new GenericFailure
                {
                    Errors = errors
                });
            }

            if (await UserHasLocationPermissions(location, GetUserId()))
            {
                location.CrowdSize++;
                await _locationService.Update(location);
                var action = new BasicLocationAction()
                {
                    LocationId = locationId
                };
                _logActionService.LogUserAction(GetUserId(), (int)ActionTypes.IncrementCrowd, action);
                return Ok(location);
            }

            errors.Add("You don't have permission.");
            return BadRequest(new GenericFailure
            {
                Errors = errors
            });
        }

        // GET: api/Locations/decrementCrowd/5
        [HttpGet("decrementCrowd/{locationId}")]
        [Authorize]
        //[AuthorizeRoles(RoleTypes.Vendor, RoleTypes.Admin)]
        public async Task<IActionResult> DecrementCrowd(string locationId)
        {
            var location = await _locationService.Get(locationId);
            var errors = new List<string>();
            if (location == null)
            {
                errors.Add("Location ID is invalid");
                return BadRequest(new GenericFailure
                {
                    Errors = errors
                });
            }
            if (GetUserRole() == RoleTypes.Admin || await UserHasLocationPermissions(location, HttpContext.GetUserId()))
            {
                location.CrowdSize--;
                await _locationService.Update(location);
                var action = new BasicLocationAction()
                {
                    LocationId = locationId
                };
                _logActionService.LogUserAction(GetUserId(), (int)ActionTypes.DecrementCrowd, action);
                return Ok(location);
            }

            errors.Add("You don't have permission.");
            return BadRequest(new GenericFailure
            {
                Errors = errors
            });
        }

        private string GetUserId()
        {
            if (_httpContextAccessor.HttpContext.User.Claims.Any())
            {
                return _httpContextAccessor.HttpContext.User.Claims.Single(u => u.Type == "Id").Value;
            }
            return string.Empty;
        }

        private string GetUserRole()
        {
            if (_httpContextAccessor.HttpContext.User.Claims.Any())
            {
                return _httpContextAccessor.HttpContext.User.Claims.Single(u => u.Type == "Role").Value;
            }
            return string.Empty;
        }

        private PoppinUser GetUserProfile(string id)
								{
            return _userService.GetUserById(id).Result;
								}

        private async Task<bool> UserHasLocationPermissions(PoppinLocation loc, string userId)
        {
            var vendor = await _vendorService.GetVendorById(loc.VendorId);

            if (vendor != null)
            {
                return vendor.MemberIds.Contains(userId);
            }
            return false;
        }
    }
}
