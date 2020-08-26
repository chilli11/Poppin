using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Poppin.Contracts.Responses;
using Poppin.Extensions;
using Poppin.Interfaces;
using Poppin.Models.Identity;
using Poppin.Models.Tracking;
using Segment;

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

								public ProfileController(IIdentityService identityService, IUserService userService, IHttpContextAccessor httpContextAccessor, ILogActionService logActionService)
								{
												_identityService = identityService;
												_userService = userService;
												_httpContextAccessor = httpContextAccessor;
												_logActionService = logActionService;
								}


								// GET: api/<ProfileController>
								[HttpGet]
								public async Task<IActionResult> Get()
								{
												var id = GetUserId();
												if (id == null)
												{
																var errors = new List<string>();
																errors.Add("User not found");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}
												var user = await _userService.GetUserById(id);
												if (user == null)
												{
																var u = await _identityService.GetUserById(id);
																user = new PoppinUser(u.User);
																_userService.AddUser(user);
												}
												return Ok(user);
								}

								// GET api/<ProfileController>/5
								[HttpGet("{id}")]
								public async Task<IActionResult> Get(string id)
								{
												// var isAdmin = _httpContextAccessor.HttpContext.User.Claims.Single(u => u.Type == "Role").Value == RoleTypes.Admin;

												var user = await _userService.GetUserById(id);
												if (user == null)
												{
																var u = await _identityService.GetUserById(id);
																if (u == null)
																{
																				var errors = new List<string>();
																				errors.Add("User not found");
																				return BadRequest(new GenericFailure
																				{
																								Errors = errors
																				});
																}
																user = new PoppinUser(u.User);
																_userService.AddUser(user);
												}
												return Ok(user);
								}

								/// <summary>
								/// Add a location to the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("favorites/add/{locationid}")]
								public IActionResult AddFavorite(string locationId)
								{
												var id = GetUserId(SegmentIOKeys.Actions.AddFavorite);
												var action = new BasicLocationAction()
												{
																LocationId = locationId
												};
												_logActionService.LogUserAction(HttpContext.GetUserId(), (int)ActionTypes.SaveFavorite, action);

												// Segment.io Analytics
												Analytics.Client.Track(id, SegmentIOKeys.Actions.AddFavorite);

												return Ok(_userService.AddFavorite(id, locationId));
								}

								/// <summary>
								/// Add a location to the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("favorites/remove/{locationid}")]
								public IActionResult RemoveFavorite(string locationId)
								{
												var id = GetUserId(SegmentIOKeys.Actions.RemoveFavorite);
												var action = new BasicLocationAction()
												{
																LocationId = locationId
												};
												_logActionService.LogUserAction(HttpContext.GetUserId(), (int)ActionTypes.RemoveFavorite, action);

												// Segment.io Analytics
												Analytics.Client.Track(id, SegmentIOKeys.Actions.RemoveFavorite);

												return Ok(_userService.RemoveFavorite(id, locationId));
								}

								/// <summary>
								/// Add a location to the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("hide/{locationid}")]
								public IActionResult HideLocation(string locationId)
								{
												var id = GetUserId(SegmentIOKeys.Actions.HideLocation);
												var action = new BasicLocationAction()
												{
																LocationId = locationId
												};
												_logActionService.LogUserAction(HttpContext.GetUserId(), (int)ActionTypes.HideLocation, action);

												// Segment.io Analytics
												Analytics.Client.Track(id, SegmentIOKeys.Actions.HideLocation);

												return Ok(_userService.HideLocation(id, locationId));
								}

								/// <summary>
								/// Add a location to the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("unhide/{locationid}")]
								public IActionResult UnhideLocation(string locationId)
								{
												var id = GetUserId(SegmentIOKeys.Actions.UnhideLocation);
												var action = new BasicLocationAction()
												{
																LocationId = locationId
												};
												_logActionService.LogUserAction(HttpContext.GetUserId(), (int)ActionTypes.UnhideLocation, action);

												// Segment.io Analytics
												Analytics.Client.Track(id, SegmentIOKeys.Actions.UnhideLocation);

												return Ok(_userService.UnhideLocation(id, locationId));
								}

								// PUT api/<ProfileController>/5
								[HttpPut("{id}")]
								public void Put(string id, [FromBody] string value)
								{

												// Segment.io Analytics
												_identityService.Identify(id, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.UpdateProfile);
												Analytics.Client.Track(id, SegmentIOKeys.Actions.UpdateProfile);

								}

								// DELETE api/<ProfileController>/5
								[HttpDelete("{id}")]
								public void Delete(string id)
								{

												// Segment.io Analytics
												_identityService.Identify(id, SegmentIOKeys.Categories.Identity, SegmentIOKeys.Actions.UpdateProfile);
												Analytics.Client.Track(id, SegmentIOKeys.Actions.UpdateProfile);
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
				}
}
