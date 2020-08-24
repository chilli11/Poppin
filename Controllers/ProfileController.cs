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
								public void AddFavorite(string locationId)
								{

												var action = new BasicLocationAction()
												{
																LocationId = locationId
												};
												_logActionService.LogUserAction(HttpContext.GetUserId(), (int)ActionTypes.SaveFavorite, action);
												_userService.AddFavorite(GetUserId(), locationId);
								}

								/// <summary>
								/// Add a location to the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("favorites/remove/{locationid}")]
								public void RemoveFavorite(string locationId)
								{

												var action = new BasicLocationAction()
												{
																LocationId = locationId
												};
												_logActionService.LogUserAction(HttpContext.GetUserId(), (int)ActionTypes.RemoveFavorite, action);
												_userService.AddFavorite(GetUserId(), locationId);
								}

								/// <summary>
								/// Add a location to the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("hide/{locationid}")]
								public void HideLocation(string locationId)
								{

												var action = new BasicLocationAction()
												{
																LocationId = locationId
												};
												_logActionService.LogUserAction(HttpContext.GetUserId(), (int)ActionTypes.HideLocation, action);
												_userService.AddFavorite(GetUserId(), locationId);
								}

								/// <summary>
								/// Add a location to the user's favorites list
								/// </summary>
								/// <param name="locationId"></param>
								[HttpGet("unhide/{locationid}")]
								public void UnhideLocation(string locationId)
								{

												var action = new BasicLocationAction()
												{
																LocationId = locationId
												};
												_logActionService.LogUserAction(HttpContext.GetUserId(), (int)ActionTypes.UnhideLocation, action);
												_userService.AddFavorite(GetUserId(), locationId);
								}

								// PUT api/<ProfileController>/5
								[HttpPut("{id}")]
								public void Put(int id, [FromBody] string value)
								{
								}

								// DELETE api/<ProfileController>/5
								[HttpDelete("{id}")]
								public void Delete(int id)
								{
								}

								private string GetUserId()
								{
												if (_httpContextAccessor.HttpContext.User == null)
												{
																return string.Empty;
												}

												return _httpContextAccessor.HttpContext.User.Claims.Single(u => u.Type == "Id").Value;
								}
				}
}
