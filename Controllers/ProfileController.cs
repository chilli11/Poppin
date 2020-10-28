﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.GeoJsonObjectModel;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using Poppin.Contracts.Requests;
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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Poppin.Controllers
{
				[Authorize]
				[Route("api/[controller]")]
				[ApiController]
				public class ProfileController : PoppinBaseController
				{
								private readonly GeometryFactory geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

								public ProfileController(
												IIdentityService identityService,
												IUserService userService,
												ILogActionService logActionService,
												ILocationService locationService,
												IVendorService vendorService)
								{
												_identityService = identityService;
												_userService = userService;
												_logActionService = logActionService;
												_locationService = locationService;
												_vendorService = vendorService;
								}


								// GET: api/<ProfileController>
								/// <summary>
								/// Get current user's profile info
								/// </summary>
								/// <returns></returns>
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
                        
												Track(id, SegmentIOKeys.Actions.AddFavorite);
												return Ok(await GetPoppinUserResult(user));
								}

								// GET api/<ProfileController>/5
								/// <summary>
								/// Get specific user profile info
								/// Only for site Admins
								/// </summary>
								/// <param name="id"></param>
								/// <returns></returns>
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

												Track(GetUserId(SegmentIOKeys.Actions.ViewUserProfile), SegmentIOKeys.Actions.AddFavorite);
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
												Track(id, SegmentIOKeys.Actions.AddFavorite);
												return Ok(recentLocations);
								}

								/// <summary>
								/// Add a location to the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("favorites/add/{locationid}")]
								public async Task<IActionResult> AddFavorite(string locationId)
								{
												if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
												{
																return BadRequest(new GenericFailure
																{
																				Errors = new[] { "The location ID was invalid." }
																});
												}

												var id = GetUserId(SegmentIOKeys.Actions.AddFavorite);
												var action = new Dictionary<string, string>()
												{
																{ "LocationId", locationId }
												};

												// Tracking
												Track(id, SegmentIOKeys.Actions.AddFavorite);
												_logActionService.LogUserAction(id, SegmentIOKeys.Actions.AddFavorite, action);

												try
												{
																await _userService.AddFavorite(id, locationId);
																return Ok();
												}
												catch (Exception ex)
												{
																var errors = new List<string>();
																errors.Add(ex.Message);
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}
								}

								/// <summary>
								/// Remove a location from the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("favorites/remove/{locationid}")]
								public async Task<IActionResult> RemoveFavorite(string locationId)
								{
												if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
												{
																return BadRequest(new GenericFailure
																{
																				Errors = new[] { "The location ID was invalid." }
																});
												}

												var id = GetUserId(SegmentIOKeys.Actions.RemoveFavorite);
												var action = new Dictionary<string, string>()
												{
																{ "LocationId", locationId }
												};

												// Segment.io Analytics
												Track(id, SegmentIOKeys.Actions.RemoveFavorite);
												_logActionService.LogUserAction(id, SegmentIOKeys.Actions.RemoveFavorite, action);

												try
												{
																await _userService.RemoveFavorite(id, locationId);
																return Ok();
												}
												catch (Exception ex)
												{
																var errors = new List<string>();
																errors.Add(ex.Message);
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}
								}

								/// <summary>
								/// Hide a location from the user's search results
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("hide/{locationid}")]
								public IActionResult HideLocation(string locationId)
								{
												if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
												{
																return BadRequest(new GenericFailure
																{
																				Errors = new[] { "The location ID was invalid." }
																});
												}

												var id = GetUserId(SegmentIOKeys.Actions.HideLocation);
												var action = new Dictionary<string, string>()
												{
																{ "LocationId", locationId }
												};

												// Segment.io Analytics
												Track(id, SegmentIOKeys.Actions.HideLocation);
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
												if (!Regex.IsMatch(locationId, "^[a-zA-Z0-9]{24}$"))
												{
																return BadRequest(new GenericFailure
																{
																				Errors = new[] { "The location ID was invalid." }
																});
												}

												var id = GetUserId(SegmentIOKeys.Actions.UnhideLocation);
												var action = new Dictionary<string, string>()
												{
																{ "LocationId", locationId }
												};

												// Segment.io Analytics
												Track(id, SegmentIOKeys.Actions.UnhideLocation);
												_logActionService.LogUserAction(id, SegmentIOKeys.Actions.UnhideLocation, action);

												return Ok(_userService.UnhideLocation(id, locationId));
								}

								// PUT api/<ProfileController>/5
								[HttpPut()]
								public async Task<IActionResult> Put(PoppinUserRequest request)
								{
												var user = await GetUserProfile(request.UserId);
												if (user == null)
												{
																return BadRequest(new GenericFailure
																{
																				Errors = new[] { "User not found" }
																});
												}

												// log specific changes with UserLog

												try
												{
																// Segment.io Analytics
																_identityService.Identify(user.UserId, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.UpdateProfile);
																Track(user.UserId, SegmentIOKeys.Actions.UpdateProfile);
																user.Merge(request);
																await _userService.UpdateUser(user);
																return Ok(user);
												}
												catch (Exception ex)
												{
																var errors = new List<string>();
																errors.Add(ex.Message);
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}
								}

								// DELETE api/<ProfileController>/5
								[HttpDelete("{id}")]
								public void Delete(string id)
								{
												// Segment.io Analytics
												_identityService.Identify(id, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.UpdateProfile);
												Track(id, SegmentIOKeys.Actions.UpdateProfile);
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
												Track(id, SegmentIOKeys.Actions.UpdateGeo);

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
																				Track(id, SegmentIOKeys.Actions.Checkin);

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

								private async Task<PoppinUserResult> GetPoppinUserResult(PoppinUser user)
								{
												return new PoppinUserResult()
												{
																User = user,
																Favorites = await user.GetFavorites(_locationService),
																Hidden = await user.GetHidden(_locationService),
																Vendors = await user.GetVendors(_vendorService)
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
