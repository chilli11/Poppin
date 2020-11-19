﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver.GeoJsonObjectModel;
using Poppin.Contracts.Requests;
using Poppin.Contracts.Responses;
using Poppin.Extensions;
using Poppin.Interfaces;
using Poppin.Models;
using Poppin.Models.Identity;
using Poppin.Models.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Poppin.Controllers
{
				[Route("api/[controller]")]
    [ApiController]
    public class LocationsController : PoppinBaseController
    {
        private List<PoppinLocation> _searchedLocations = new List<PoppinLocation>();

        public LocationsController(
            ILocationService locationService,
            IUserService userService,
            IYelpService yelpService,
            IVendorService vendorService,
            ILogActionService logActionService,
            ILogger<LocationsController> logger,
            IIdentityService identityService,
            IHEREGeocoder hereGeocoder)
        {
            _userService = userService;
            _locationService = locationService;
            _yelpService = yelpService;
            _vendorService = vendorService;
            _logActionService = logActionService;
            _identityService = identityService;
            _hereGeocoder = hereGeocoder;
            _logger = logger;
        }

        /// <summary>
        /// Gets location info based on ID passed as URL param.
        /// </summary>
        /// <param name="locationId"></param>
        /// <returns></returns>
        // GET: api/Locations/5
        [HttpGet("{locationId}")]
        public IActionResult Get(string locationId)
        {
            if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
            {
                var errors = new[] { "The location ID was invalid." };
                _logger.LogError("Location Search Failed: {errors}", "Location ID improperly formatted.");
                return BadRequest(new GenericFailure
                {
                    Errors = errors
                });
            }

            var location = GetLocation(locationId);
            if (location == null)
            {
                var errors = new[] { "Location ID is invalid" };
                _logger.LogError("Location Search Failed: {errors}", "Location ID not found.");
                return BadRequest(new GenericFailure
                {
                    Errors = errors
                });
            }

            var action = new Dictionary<string, string>()
            {
                { "LocationId", location.Id }
            };
            _logActionService.LogUserAction(GetUserId(SegmentIOKeys.Actions.ViewLocation), SegmentIOKeys.Actions.ViewLocation, action);

            _logger.LogInformation("Retrieved Location: {id}", location.Id);
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
        [HttpGet("with-yelp/{locationId}")]
        public async Task<IActionResult> GetWithYelp(string locationId)
        {
            if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
            {
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { "The location ID was invalid." }
                });
            }

            var location = GetLocation(locationId);
            if (location == null)
            {
                var errors = new List<string>();
                errors.Add("Location ID is invalid");
                return BadRequest(new GenericFailure
                {
                    Errors = errors
                });
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

            var action = new Dictionary<string, string>()
            {
                { "LocationId", location.Id }
            };
            _logActionService.LogUserAction(GetUserId(SegmentIOKeys.Actions.ViewLocation), SegmentIOKeys.Actions.ViewLocation, action);

            location.SetCrowdSize(_locationService).Wait();
            return Ok(location);
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(LocationSearchRequest search)
								{
            try
            {
                var id = GetUserId(SegmentIOKeys.Actions.Search);
                var catSlugs = await _locationService.GetCategoriesBySlug(search.Categories.Select(c => c.Slug));

                if (search.Geo.Coordinates.Length == 0)
																{
                    if (string.IsNullOrEmpty(search.Location))
																				{
                        return BadRequest(new GenericFailure
                        {
                            Errors = new[] { "`location` or `geo` parameter required" }
                        });
																				}
                    var geocode = _hereGeocoder.Geocode(new Dictionary<string, string> { { "q", search.Location } });
                    search.Geo.Coordinates = new double[] { geocode.Position["lng"], geocode.Position["lat"] };
																}

                if (search.Categories.Count > 0 && search.Categories.First().Id == null)
																{
                   
                    search.Categories = catSlugs.ToHashSet();
																}

                var locList = await _locationService.GetBySearch(search);

                if (locList.Count > 0)
                {
                    if (!string.IsNullOrEmpty(search.Term))
                    {
                        locList = locList.FindAll(l => l.Name.IndexOf(search.Term, StringComparison.InvariantCultureIgnoreCase) >= 0).ToList();
                    }

                    locList.UpdateCrowdSizes(await _locationService.GetCheckinsForLocations(locList.Select(l => l.Id)));
                }

                var actionStr = new Dictionary<string, string>()
                {
                    { "SearchTerm", search.Term },
                    { "SearchLocation", $"{search.Geo.Coordinates[0]}, {search.Geo.Coordinates[1]}" },
                    { "SearchCategories", catSlugs.ToString() }
                };

                var actionObj = new Dictionary<string, object>()
                {
                    { "SearchTerm", search.Term },
                    { "SearchLocation", search.Geo },
                    { "SearchCategories", catSlugs }
                };

                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.Search, actionStr);
                Track(id, SegmentIOKeys.Actions.Search, actionObj);

                return Ok(new PoppinSearchResponse()
                {
                    Total = locList.Count,
                    Businesses = locList,
                    SearchParams = search
                });
            }
            catch (Exception e)
            {
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { e.Message }
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

                if (location.Address.Geo == null)
                {
                    if (string.IsNullOrEmpty(location.Address.Line1) || string.IsNullOrEmpty(location.Address.City) || string.IsNullOrEmpty(location.Address.State))
                    {
                        return BadRequest(new GenericFailure
                        {
                            Errors = new[] { "missing required address parameters" }
                        });
                    }
                    location.Address.Geo = GeocodeAddress(location.Address);
                }

                await _locationService.Add(location);

                var action = new Dictionary<string, string>()
                {
                    { "LocationId", location.Id }
                };
                var id = GetUserId(SegmentIOKeys.Actions.AddLocation);
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.AddLocation, action);
                Track(id, SegmentIOKeys.Actions.AddLocation);

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

                if (location.Address.Geo == null)
                {
                    if (!string.IsNullOrEmpty(location.Address.Line1) && !string.IsNullOrEmpty(location.Address.City) && !string.IsNullOrEmpty(location.Address.State))
                    {
                        location.Address.Geo = GeocodeAddress(location.Address);
                    }
                }

                await _locationService.Update(location.Id, location);

                var action = new Dictionary<string, string>()
                {
                    { "LocationId", location.Id }
                };
                var id = GetUserId(SegmentIOKeys.Actions.UpdateLocation);
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.UpdateLocation, action);
                Track(id, SegmentIOKeys.Actions.UpdateLocation);

                _searchedLocations.Remove(location);
                _searchedLocations.Add(location);
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
            if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
            {
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { "The location ID was invalid." }
                });
            }

            if (GetUserRole() != RoleTypes.Admin)
            {
                return Unauthorized();
            }
            var location = new PoppinLocation(_location);
            try
            {
                location.LastUpdate = DateTime.UtcNow;
                await _locationService.Update(locationId, location);

                var action = new Dictionary<string, string>()
                {
                    { "LocationId", location.Id }
                };
                var id = GetUserId(SegmentIOKeys.Actions.UpdateLocation);
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.UpdateLocation, action);
                Track(id, SegmentIOKeys.Actions.UpdateLocation, action.AsDictionary());

                _searchedLocations.Remove(location);
                _searchedLocations.Add(location);
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
            if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
            {
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { "The location ID was invalid." }
                });
            }

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
            var score = string.IsNullOrEmpty(userId) ? ReliabilityScores.Vendor : ReliabilityScores.User;
            var checkin = new Checkin(locationId, userId, location.VisitLength, score);

            if (_locationService.ReconcileCheckin(checkin))
            {
                await _locationService.NewCheckin(checkin);
                await location.SetCrowdSize(_locationService);

                var action = new Dictionary<string, string>()
                {
                    { "LocationId", location.Id }
                };
                _logActionService.LogUserAction(userId, SegmentIOKeys.Actions.Checkin, action);
                Track(userId, SegmentIOKeys.Actions.Checkin, checkin.AsDictionary());
            }

            return Ok(location);
        }

        [HttpGet("geo-checkin/{locationId}")]
        public async Task<IActionResult> GeoCheckIn(string locationId)
        {
            if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
            {
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { "The location ID was invalid." }
                });
            }

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

            var checkin = new Checkin(locationId, userId, location.VisitLength, ReliabilityScores.Geo);
            await _locationService.NewCheckin(checkin);
            await location.SetCrowdSize(_locationService);

            var action = new Dictionary<string, string>()
            {
                { "LocationId", location.Id }
            };
            _logActionService.LogUserAction(userId, SegmentIOKeys.Actions.Checkin, action);
            Track(userId, SegmentIOKeys.Actions.Checkin, checkin.AsDictionary());

            return Ok(location);
        }

        // DELETE: api/Locations/5
        [HttpDelete("{locationId}")]
        [Authorize]
								//[AuthorizeRoles()]
								public IActionResult Delete(string locationId)
        {
            if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
            {
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { "The location ID was invalid." }
                });
            }

            if (GetUserRole() != RoleTypes.Admin)
												{
                return Unauthorized();
            }
            _locationService.Delete(locationId);

            var action = new Dictionary<string, string>()
            {
                { "LocationId", locationId }
            };
            var id = GetUserId(SegmentIOKeys.Actions.DeleteLocation);
            _logActionService.LogUserAction(id, SegmentIOKeys.Actions.DeleteLocation, action);
            Track(id, SegmentIOKeys.Actions.DeleteLocation);
            return Ok();
        }

        // GET: api/Locations/incrementCrowd/5
        [HttpGet("increment-crowd/{locationId}")]
        [Authorize]
        //[AuthorizeRoles(RoleTypes.Vendor, RoleTypes.Admin)]
        public async Task<IActionResult> IncrementCrowd(string locationId)
        {
            if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
            {
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { "The location ID was invalid." }
                });
            }

            var location = GetLocation(locationId);
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
                var c = new Checkin(locationId, null, location.VisitLength, ReliabilityScores.Vendor);
                await _locationService.NewCheckin(c);

                var action = new Dictionary<string, string>()
                {
                    { "LocationId", locationId }
                };
                _logActionService.LogUserAction(id, SegmentIOKeys.Actions.IncrementCrowd, action);
                Track(id, SegmentIOKeys.Actions.IncrementCrowd);

                await location.SetCrowdSize(_locationService);
                return Ok(location);
            }

            errors.Add("You don't have permission.");
            return BadRequest(new GenericFailure
            {
                Errors = errors
            });
        }

        // GET: api/Locations/decrementCrowd/5
        [HttpGet("decrement-crowd/{locationId}")]
        [Authorize]
        //[AuthorizeRoles(RoleTypes.Vendor, RoleTypes.Admin)]
        public async Task<IActionResult> DecrementCrowd(string locationId)
        {
            if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
            {
                return BadRequest(new GenericFailure
                {
                    Errors = new[] { "The location ID was invalid." }
                });
            }

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
                try
                {
                    await _locationService.InvalidateVendorCheckin(locationId);

                    var action = new Dictionary<string, string>()
                    {
                        { "LocationId", locationId }
                    };
                    _logActionService.LogUserAction(id, SegmentIOKeys.Actions.DecrementCrowd, action);
                    Track(id, SegmentIOKeys.Actions.DecrementCrowd);

                    await location.SetCrowdSize(_locationService);
                    return Ok(location);
                }
                catch (Exception ex)
																{
                    return BadRequest(new GenericFailure
                    {
                        Errors = new[] { "No eligible checkins to remove" }
                    });
																}
            }

            errors.Add("You don't have permission.");
            return BadRequest(new GenericFailure
            {
                Errors = errors
            });
        }

        private PoppinLocation GetLocation(string locationId)
        {
            var location = _searchedLocations.SingleOrDefault(l => l.Id == locationId);
            if (location == null)
            {
                location = _locationService.Get(locationId).Result;
                _searchedLocations.Add(location);
            }
            return location;
        }

        private GeoJsonPoint<GeoJson2DGeographicCoordinates> GeocodeAddress(Address locationAddress)
        {
            var address = $"{locationAddress.Line1}, {locationAddress.City}, {locationAddress.State}";
            var geocode = _hereGeocoder.Geocode(new Dictionary<string, string> { { "q", address } });
            var c = new Coord
            {
                Longitude = geocode.Position["lng"],
                Latitude = geocode.Position["lat"]
            };
            return c.ToGeoJson();
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
