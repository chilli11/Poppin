using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.GeoJsonObjectModel;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Poppin.Contracts.Responses;
using Poppin.Extensions;
using Poppin.Interfaces;
using Poppin.Models;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Identity;
using Poppin.Models.Tracking;
using Segment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Poppin.Controllers
{
				[Authorize]
				[Route("api/[controller]")]
				[ApiController]
				public class ProfileController : ControllerBase
				{
								private readonly IHttpContextAccessor _httpContextAccessor;
								private readonly IUserService _userService;
								private readonly IIdentityService _identityService;
								private readonly ILogActionService _logActionService;
								private readonly ILocationService _locationService;

								private readonly GeometryFactory geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

								public ProfileController(
												IIdentityService identityService,
												IUserService userService,
												IHttpContextAccessor httpContextAccessor,
												ILogActionService logActionService,
												ILocationService locationService)
								{
												_identityService = identityService;
												_userService = userService;
												_httpContextAccessor = httpContextAccessor;
												_logActionService = logActionService;
												_locationService = locationService;
								}


								// GET: api/<ProfileController>
								[HttpGet]
								public async Task<IActionResult> Get()
								{
												var id = GetUserId(SegmentIOKeys.Actions.ViewUserProfile);
												if (id == null)
												{
																var errors = new List<string>();
																errors.Add("User not found");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}
												var user = await GetUserProfile(id);
                        
												Analytics.Client.Track(id, SegmentIOKeys.Actions.AddFavorite);
												return Ok(GetPoppinUserResult(user));
								}

								// GET api/<ProfileController>/5
								[Authorize(Roles = RoleTypes.Admin)]
								[HttpGet("{id}")]
								public async Task<IActionResult> Get(string id)
								{
												var isAdmin = GetUserRole() == RoleTypes.Admin;

												if (!isAdmin)
												{
																return Forbid();
												}

												var user = await GetUserProfile(id);
												if (user == null)
												{
																var errors = new List<string>();
																errors.Add("User not found");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}

												Analytics.Client.Track(GetUserId(SegmentIOKeys.Actions.ViewUserProfile), SegmentIOKeys.Actions.AddFavorite);
												return Ok(GetPoppinUserResult(user));
								}

								/// <summary>
								/// Returns locations viewed since the day before
								/// </summary>
								/// <returns></returns>
								[HttpGet("recently-viewed")]
								public IActionResult GetRecentlyViewed()
								{
												var id = GetUserId(SegmentIOKeys.Actions.ViewUserProfile);
												if (id == null)
												{
																var errors = new List<string>();
																errors.Add("User not found");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}

												var recentLocations = GetRecentLocationList(id, -1, 0);
												Analytics.Client.Track(id, SegmentIOKeys.Actions.AddFavorite);
												return Ok(recentLocations);
								}

								/// <summary>
								/// Add a location to the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("favorites/add/{locationid}")]
								public IActionResult AddFavorite(string locationId)
								{
												var id = GetUserId(SegmentIOKeys.Actions.AddFavorite);

												var action = new Dictionary<string, string>()
												{
																{ "LocationId", locationId }
												};

												// Tracking
												Analytics.Client.Track(id, SegmentIOKeys.Actions.AddFavorite);
												_logActionService.LogUserAction(id, SegmentIOKeys.Actions.AddFavorite, action);

												return Ok(_userService.AddFavorite(id, locationId));
								}

								/// <summary>
								/// Remove a location from the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("favorites/remove/{locationid}")]
								public IActionResult RemoveFavorite(string locationId)
								{
												var id = GetUserId(SegmentIOKeys.Actions.RemoveFavorite);

												var action = new Dictionary<string, string>()
												{
																{ "LocationId", locationId }
												};

												// Segment.io Analytics
												Analytics.Client.Track(id, SegmentIOKeys.Actions.RemoveFavorite);
												_logActionService.LogUserAction(id, SegmentIOKeys.Actions.RemoveFavorite, action);

												return Ok(_userService.RemoveFavorite(id, locationId));
								}

								/// <summary>
								/// Hide a location from the user's search results
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("hide/{locationid}")]
								public IActionResult HideLocation(string locationId)
								{
												var id = GetUserId(SegmentIOKeys.Actions.HideLocation);

												var action = new Dictionary<string, string>()
												{
																{ "LocationId", locationId }
												};

												// Segment.io Analytics
												Analytics.Client.Track(id, SegmentIOKeys.Actions.HideLocation);
												_logActionService.LogUserAction(id, SegmentIOKeys.Actions.HideLocation, action);

												return Ok(_userService.HideLocation(id, locationId));
								}

								/// <summary>
								/// Unhide a location from the user's search results
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("unhide/{locationid}")]
								public IActionResult UnhideLocation(string locationId)
								{
												var id = GetUserId(SegmentIOKeys.Actions.UnhideLocation);
												var action = new Dictionary<string, string>()
												{
																{ "LocationId", locationId }
												};

												// Segment.io Analytics
												Analytics.Client.Track(id, SegmentIOKeys.Actions.UnhideLocation);
												_logActionService.LogUserAction(id, SegmentIOKeys.Actions.UnhideLocation, action);

												return Ok(_userService.UnhideLocation(id, locationId));
								}

								// PUT api/<ProfileController>/5
								[HttpPut()]
								public IActionResult Put(PoppinUser user)
								{
												// Segment.io Analytics
												_identityService.Identify(user.UserId, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.UpdateProfile);
												Analytics.Client.Track(user.UserId, SegmentIOKeys.Actions.UpdateProfile);
												var oldUser = GetUserProfile(user.UserId);
												if (oldUser == null)
												{
																var errors = new List<string>();
																errors.Add("User not found");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}

												// log specific changes with UserLog

												return Ok(_userService.UpdateUser(user));
								}

								// PUT api/<ProfileController>/5
								[HttpPut("{id}")]
								public IActionResult Put(string id, PoppinUser user)
								{
												// Segment.io Analytics
												_identityService.Identify(id, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.UpdateProfile);
												Analytics.Client.Track(id, SegmentIOKeys.Actions.UpdateProfile);
												var oldUser = GetUserProfile(id);
												if (oldUser == null)
												{
																var errors = new List<string>();
																errors.Add("User not found");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}

												// log specific changes with UserLog

												return Ok(_userService.UpdateUser(id, user));
								}

								// DELETE api/<ProfileController>/5
								[HttpDelete("{id}")]
								public void Delete(string id)
								{
												// Segment.io Analytics
												_identityService.Identify(id, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.UpdateProfile);
												Analytics.Client.Track(id, SegmentIOKeys.Actions.UpdateProfile);
								}

								/// <summary>
								/// Update User location
								/// </summary>
								[HttpPost("update-geo")]
								public void UpdateGeo(GeoCoords geoJson)
								{
												var id = GetUserId(SegmentIOKeys.Actions.UpdateGeo);
												var coords = new GeoJson2DGeographicCoordinates(geoJson.Coordinates[0], geoJson.Coordinates[1]);
												var action = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(coords).AsStringDictionary();
												_logActionService.LogUserAction(id, SegmentIOKeys.Actions.UpdateGeo, (Dictionary<string,string>)action);
												Analytics.Client.Track(id, SegmentIOKeys.Actions.UpdateGeo);

												var recent = GetRecentLocationList(id, 0, -2);
												if (recent.Count > 0)
												{
																if (recent.Count > 1)
																{
																				recent.Sort((a, b) => CompareDistance(geoJson, a, b));
																}
																var closest = recent.First();
																if (GetDistance(
																				new[] { geoJson.Coordinates[0], geoJson.Coordinates[1] },
																				new[] { closest.Address.Geo.Coordinates.Longitude, closest.Address.Geo.Coordinates.Latitude }
																) < 50)
																{
																				var checkinAction = new Dictionary<string, string>()
																				{
																								{ "LocationId", closest.Id }
																				};
																				_logActionService.LogUserAction(id, SegmentIOKeys.Actions.Checkin, checkinAction);
																				Analytics.Client.Track(id, SegmentIOKeys.Actions.Checkin);

																				var checkin = new Checkin(closest.Id, id, closest.VisitLength, ReliabilityScores.Geo);
																				_locationService.NewCheckin(checkin);
																}
												}

								}

								/// <summary>
								/// https://docs.microsoft.com/en-us/ef/core/modeling/spatial
								/// </summary>
								/// <param name="geoJson"></param>
								/// <param name="a"></param>
								/// <param name="b"></param>
								/// <returns>-1|0|1</returns>
								private int CompareDistance(GeoCoords geoJson, PoppinLocation a, PoppinLocation b)
								{
												var currentLoc = geometryFactory.CreatePoint(new Coordinate(geoJson.Coordinates[0], geoJson.Coordinates[1])).ProjectTo(4326);
												var pointA = geometryFactory.CreatePoint(new Coordinate(a.Address.Geo.Coordinates.Longitude, a.Address.Geo.Coordinates.Latitude)).ProjectTo(4326);
												var pointB = geometryFactory.CreatePoint(new Coordinate(b.Address.Geo.Coordinates.Longitude, b.Address.Geo.Coordinates.Latitude)).ProjectTo(4326);
												return currentLoc.Distance(pointA).CompareTo(pointB);
								}

								/// <summary>
								/// https://docs.microsoft.com/en-us/ef/core/modeling/spatial
								/// </summary>
								/// <param name="a"></param>
								/// <param name="b"></param>
								/// <returns>Distance in meters</returns>
								private double GetDistance(double[] a, double[] b)
								{
												var pointA = geometryFactory.CreatePoint(new Coordinate(a[0], a[1])).ProjectTo(4326);
												var pointB = geometryFactory.CreatePoint(new Coordinate(b[0], b[1])).ProjectTo(4326);
												return pointA.Distance(pointB);
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

								private async Task<PoppinUser> GetUserProfile(string id)
								{
												var user = await _userService.GetUserById(id);
												if (user == null)
												{
																var u = await _identityService.GetUserById(id);
																if (u != null)
																{
																				user = new PoppinUser(u.User);
																				_userService.AddUser(user);
																}
												}
												return user;
								}

								private PoppinUserResult GetPoppinUserResult(PoppinUser user)
								{
												return new PoppinUserResult()
												{
																User = user,
																Favorites = user.GetFavorites(_locationService).Result,
																Hidden = user.GetHidden(_locationService).Result,
												};
								}

								private List<PoppinLocation> GetRecentLocationList(string id, int dayOffset, int hourOffset)
								{
												if (hourOffset > 0) hourOffset = hourOffset * -1;
												if (dayOffset > 0) dayOffset = dayOffset * -1;

												var startDay = DateTime.Today;
												if (hourOffset == 0)
												{
																startDay = startDay.AddDays(dayOffset);
												}

												var logs = _logActionService.GetUserActivity(id, startDay);
												if (hourOffset < 0) logs = logs.Where(l => l.Date > startDay.AddHours(hourOffset)).ToList();
												var recent = logs.SelectMany(l => l.Entries.Where(e => e.ActionType == SegmentIOKeys.Actions.ViewLocation)).Select(a => a.Action);
												return _locationService.GetMany(recent.Select(a => a["LocationId"])).Result;
								}
				}
}
