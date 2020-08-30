﻿using System;
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
using Segment;

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
        private readonly IIdentityService _identityService;

        private List<PoppinLocation> _searchedLocations = new List<PoppinLocation>();

        public LocationsController(
            IHttpContextAccessor httpContextAccessor,
            ILocationService locationService,
            IUserService userService,
            IYelpService yelpService,
            IVendorService vendorService,
            ILogActionService logActionService,
            IIdentityService identityService)
        {
            _httpContextAccessor = httpContextAccessor;
            _userService = userService;
            _locationService = locationService;
            _yelpService = yelpService;
            _vendorService = vendorService;
            _logActionService = logActionService;
            _identityService = identityService;
        }

        /// <summary>
        /// Gets location info based on ID passed as URL param.
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        // GET: api/Locations/5
        [HttpGet("{locationId}", Name = "Get")]
        public async Task<IActionResult> Get(string locationId)
        {
            var location = _searchedLocations.Find(l => l.Id == locationId);
            if (location == null)
            {
                location = await _locationService.Get(locationId);
                if (location == null)
                {
                    var errors = new List<string>();
                    errors.Add("Location ID is invalid");
                    return BadRequest(new GenericFailure
                    {
                        Errors = errors
                    });
                }
                _searchedLocations.Add(location);
            }

            var action = new BasicLocationAction()
            {
                LocationId = location.Id
            };
            _logActionService.LogUserAction(GetUserId(SegmentIOKeys.Actions.ViewLocation), SegmentIOKeys.Actions.ViewLocation, action);

            location.SetCrowdSize(_locationService).Wait();
            return Ok(location);
        }

        /// <summary>
        /// Gets location info based on ID passed as URL param.
        /// Includes <c>YelpDetails</c> if <c>YelpId</c> is available
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        // GET: api/Locations/5
        [HttpGet("with-yelp/{locationId}", Name = "Get")]
        public async Task<IActionResult> GetWithYelp(string locationId)
        {
            var location = _searchedLocations.Find(l => l.Id == locationId);
            if (location == null)
            {
                location = await _locationService.Get(locationId);
                if (location == null)
                {
                    var errors = new List<string>();
                    errors.Add("Location ID is invalid");
                    return BadRequest(new GenericFailure
                    {
                        Errors = errors
                    });
                }
            }
            if (location.YelpDetails == null)
            {
                if (!string.IsNullOrEmpty(location.YelpId))
                {
                    _searchedLocations.Remove(location);
                    location.YelpDetails = await _yelpService.GetBusiness(location.YelpId);
                    _searchedLocations.Add(location);
                }
            }

            var action = new BasicLocationAction()
            {
                LocationId = location.Id
            };
            _logActionService.LogUserAction(GetUserId(SegmentIOKeys.Actions.ViewLocation), SegmentIOKeys.Actions.ViewLocation, action);

            location.SetCrowdSize(_locationService).Wait();
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
                var id = GetUserId(SegmentIOKeys.Actions.Search);
                var yelpSearchResponse = await _yelpService.GetBusinessSearch(searchParams);
                var locList = new List<PoppinLocation>();

                if (yelpSearchResponse.Total > 0)
                {
                    locList = await _locationService.GetByYelpList(yelpSearchResponse.Businesses.Select(y => y.Id));

                    //Hidden locations: Implement in v2
                    //if (id != string.Empty)
                    //{
                    //    var profile = GetUserProfile(id);
                    //    if (profile.Hidden != null && profile.Hidden.Any())
                    //    {
                    //        locList = locList.Where(l => !profile.Hidden.Contains(l.Id)).ToList();
                    //    }
                    //}

                    locList.ForEach(l => l.YelpDetails = yelpSearchResponse.Businesses.FirstOrDefault(yb => yb.Id == l.YelpId));
                    _searchedLocations.AddRange(locList.Where(l => !_searchedLocations.Any(sl => sl.Id == l.Id)));
                    locList.ForEach(l => l.SetCrowdSize(_locationService).Wait());
                }

                var action = new SearchAction()
                {
                    SearchTerm = searchParams.term,
                    SearchLocation = searchParams.location,
                    SearchCategories = searchParams.categories
                };
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.Search, action);
                Analytics.Client.Track(id, SegmentIOKeys.Actions.Search);

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
            location.LastUpdate = DateTime.UtcNow;

            if (isExisting == null)
            {
                await _locationService.Add(location);
                var action = new BasicLocationAction()
                {
                    LocationId = location.Id
                };
                var id = GetUserId(SegmentIOKeys.Actions.AddLocation);
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.AddLocation, action);
                Analytics.Client.Track(id, SegmentIOKeys.Actions.AddLocation);

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
                location.LastUpdate = DateTime.UtcNow;
                await _locationService.Update(location);
                var action = new BasicLocationAction()
                {
                    LocationId = location.Id
                };
                var id = GetUserId(SegmentIOKeys.Actions.UpdateLocation);
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.UpdateLocation, action);
                Analytics.Client.Track(id, SegmentIOKeys.Actions.UpdateLocation);

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
                location.LastUpdate = DateTime.UtcNow;
                await _locationService.Update(locationId, location);
                var action = new BasicLocationAction()
                {
                    LocationId = locationId
                };
                var id = GetUserId(SegmentIOKeys.Actions.UpdateLocation);
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.UpdateLocation, action);
                Analytics.Client.Track(id, SegmentIOKeys.Actions.UpdateLocation);

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

        [HttpGet("checkin/{locationId}")]
        public async Task<IActionResult> UserCheckIn(string locationId)
								{
            var userId = GetUserId(SegmentIOKeys.Actions.Checkin);
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
            var checkin = new Checkin(locationId, userId, location.VisitLength);

            var action = new BasicLocationAction()
            {
                LocationId = location.Id
            };
            _logActionService.LogUserAction(userId, SegmentIOKeys.Actions.Checkin, action);

            return Ok(_locationService.NewCheckin(checkin));
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
            var id = GetUserId(SegmentIOKeys.Actions.DeleteLocation);
            _logActionService.LogUserAction(id, SegmentIOKeys.Actions.DeleteLocation, action);
            Analytics.Client.Track(id, SegmentIOKeys.Actions.DeleteLocation);
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

            var id = GetUserId(SegmentIOKeys.Actions.IncrementCrowd);
            if (GetUserRole() == RoleTypes.Admin || await UserHasLocationPermissions(location, id))
            {
                location.CrowdSize++;
                await _locationService.Update(location);
                var action = new BasicLocationAction()
                {
                    LocationId = locationId
                };
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.IncrementCrowd, action);
                Analytics.Client.Track(id, SegmentIOKeys.Actions.IncrementCrowd);
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

            var id = GetUserId(SegmentIOKeys.Actions.DecrementCrowd);
            if (GetUserRole() == RoleTypes.Admin || await UserHasLocationPermissions(location, id))
            {
                location.CrowdSize--;
                await _locationService.Update(location);
                var action = new BasicLocationAction()
                {
                    LocationId = locationId
                };
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.DecrementCrowd, action);
                Analytics.Client.Track(id, SegmentIOKeys.Actions.DecrementCrowd);
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
                var id = _httpContextAccessor.HttpContext.User.Claims.Single(u => u.Type == "Id").Value;
                _identityService.Identify(id, SegmentIOKeys.Categories.Identity, "GetUserId");
                return id;
            }
            return string.Empty;
        }

        private string GetUserId(string action)
        {
            if (_httpContextAccessor.HttpContext.User.Claims.Any())
            {
                var id = _httpContextAccessor.HttpContext.User.Claims.Single(u => u.Type == "Id").Value;
                _identityService.Identify(id, SegmentIOKeys.Categories.Identity, action);
                return id;
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
            PoppinUser profile = _userService.GetUserById(id).Result;
            if (profile == null)
												{
                _userService.AddUser(new PoppinUser(_identityService.GetUserById(id).Result.User));
                profile = _userService.GetUserById(id).Result;
            }
            return profile;
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
