using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Poppin.Contracts.Requests;
using Poppin.Contracts.Responses;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Identity;
using Poppin.Models.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Poppin.Controllers
{
				[Route("api/[controller]")]
				[Authorize]
				[ApiController]
				public class VendorsController : ControllerBase
				{
								private readonly IVendorService _vendorService;
        private readonly ILocationService _locationService;
        private readonly IUserService _userService;
								private readonly IIdentityService _identityService;

								private List<Vendor> _vendors = new List<Vendor>();

								public VendorsController(
												IVendorService vendorService,
												ILocationService locationService,
												IUserService userService,
												IIdentityService identityService)
								{
												_vendorService = vendorService;
												_locationService = locationService;
												_userService = userService;
												_identityService = identityService;
								}

								// GET: api/Vendors/5
								[Authorize(Policy = "Admin")]
								[HttpGet]
								public async Task<IActionResult> Get()
								{
												var vendors = await _vendorService.GetAll();
												if (vendors == null)
												{
																var errors = new List<string>();
																errors.Add("No Results Found");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}
												return Ok(vendors);
								}

								// GET: api/Vendors/5
								[HttpGet("{vendorId}")]
								public async Task<IActionResult> Get(string vendorId)
								{			
												var vendor = _vendors.Find(v => v.Id == vendorId);
												if (vendor == null)
												{
																vendor = await _vendorService.GetVendorById(vendorId);
												}
												if (vendor == null)
												{
																var errors = new List<string>();
																errors.Add("Vendor ID is invalid");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}

												var userId = GetUserId(SegmentIOKeys.Actions.ViewVendor);
												if (GetUserRole() == "Admin" || vendor.AdminIds.Contains(userId) || vendor.MemberIds.Contains(userId))
												{
																var vResult = new VendorResult
																{
																				Vendor = vendor,
																				SubVendors = vendor.GetSubVendors(_vendorService),
																				Locations = vendor.GetLocations(_locationService),
																				Admins = vendor.GetAdmins(_userService),
																				Members = vendor.GetMembers(_userService)
																};
																if (vendor.ParentVendorId != null)
																{
																				vResult.Parent = await _vendorService.GetVendorById(vendor.ParentVendorId);
																}
																return Ok(vResult);
												}
												return Unauthorized();
								}

								// GET: api/Vendors/5
								[HttpPost("get-by-list")]
								public async Task<IActionResult> GetByIds(IEnumerable<string> vendorIds)
								{
												if (_vendors.Count > 0)
												{
																var vendors = _vendors.Where(v => vendorIds.Contains(v.Id)).ToList();
																vendorIds = vendorIds.Where(id => !vendors.Any(v => v.Id == id));
																vendors.AddRange(await _vendorService.GetVendorsByIds(vendorIds));
																if (vendors == null)
																{
																				var errors = new List<string>();
																				errors.Add("No Results Found");
																				return BadRequest(new GenericFailure
																				{
																								Errors = errors
																				});
																}
																return Ok(vendors);
												}
												else
												{
																var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
																var vendors = await _vendorService.GetVendorsByIds(vendorIds);
																if (vendors == null)
																{
																				var errors = new List<string>();
																				errors.Add("No Results Found");
																				return BadRequest(new GenericFailure
																				{
																								Errors = errors
																				});
																}

																if (GetUserRole() == "Admin")
																{
																				return Ok(vendors);
																}
																return Ok(vendors.Where(v => v.AdminIds.Contains(userId) || v.MemberIds.Contains(userId)));
												}
								}

								// GET: api/Vendors/5
								[HttpPost("get-by-search")]
								public async Task<IActionResult> GetBySearch(string searchTerm)
								{
												var vendors = await _vendorService.GetVendorsBySearch(searchTerm);
												if (vendors == null)
												{
																var errors = new List<string>();
																errors.Add("No Results Found");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}
												var leftovers = vendors.Where(v => !_vendors.Any(_v => v.Id == _v.Id));
												var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
												_vendors.AddRange(leftovers);

												if (GetUserRole() == "Admin")
												{
																return Ok(vendors);
												}
												return Ok(vendors.Where(v => v.AdminIds.Contains(userId) || v.MemberIds.Contains(userId)));
								}

								// POST api/Vendors
								[Authorize(Policy = "Admin")]
								[HttpPost]
								public async Task<IActionResult> Post(PoppinVendorRequest newVendor)
								{
												var vendor = new Vendor(newVendor);
												var isExisting = await _vendorService.CheckExists(vendor);
												vendor.LastUpdate = DateTime.UtcNow;

												if (isExisting == null)
												{
																await _vendorService.AddVendor(vendor);
																_vendors.Add(vendor);
																return CreatedAtAction("Post", vendor);
												}
												return Ok(isExisting);
								}

								// PUT api/Vendors/5
								[HttpPut]
								public async Task<IActionResult> Put(PoppinVendorRequest newVendor)
								{
												var vendor = new Vendor(newVendor);
												var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
												if (GetUserRole() == "Admin" || vendor.AdminIds.Contains(userId))
												{
																try
																{
																				vendor.LastUpdate = DateTime.UtcNow;
																				await _vendorService.UpdateVendor(vendor);
																				var ind = _vendors.FindIndex(v => v.Id == vendor.Id);
																				if (ind > -1)
																				{
																								_vendors.RemoveAt(ind);
																				}
																				_vendors.Add(vendor);
																				return Ok();
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
												return Unauthorized();
								}

								// PUT api/Vendors/5
								[HttpPut("{vendorId}")]
								public async Task<IActionResult> Put(string vendorId, PoppinVendorRequest newVendor)
								{
												var vendor = new Vendor(newVendor);
												var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
												if (GetUserRole() == "Admin" || vendor.AdminIds.Contains(userId))
												{
																try
																{
																				vendor.LastUpdate = DateTime.UtcNow;
																				await _vendorService.UpdateVendor(vendorId, vendor);
																				var ind = _vendors.FindIndex(v => v.Id == vendorId);
																				if (ind > -1)
																				{
																								_vendors.RemoveAt(ind);
																				}
																				_vendors.Add(vendor);
																				return Ok();
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
												return Unauthorized();
								}

								//POST api/Vendors/5
								[HttpPost("add-member/{vendorId}")]
								public async Task<IActionResult> AddMember(string vendorId, VendorMemberRequest newMember)
								{
												var user = await GetUserProfile(newMember);
												var vendor = _vendors.Find(v => v.Id == vendorId);
												if (vendor == null)
												{
																vendor = await _vendorService.GetVendorById(vendorId);
												}
												if (vendor == null || user == null)
												{
																var errors = new List<string>();
																errors.Add("Vendor or User ID is invalid");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}

												var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
												if (GetUserRole() == "Admin" || vendor.AdminIds.Contains(userId))
												{
																try
																{
																				if (newMember.Role == RoleTypes.Admin)
																				{
																								vendor.AdminIds.Add(user.UserId);
																								vendor.MemberIds.Remove(user.UserId);
																				}
																				else
																				{
																								vendor.MemberIds.Add(user.UserId);
																				}
																				if (user.VendorIds == null)
																				{
																								user.VendorIds = new HashSet<string>();
																				}
																				user.VendorIds.Add(vendorId);

																				await _vendorService.UpdateVendor(vendor);
																				await _userService.UpdateUser(user.UserId, user);
																				return Ok();
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
												return Unauthorized();
								}

								//POST api/Vendors/5
								[HttpPost("remove-member/{vendorId}")]
								public async Task<IActionResult> RemoveMember(string vendorId, VendorMemberRequest newMember)
								{
												var user = await GetUserProfile(newMember);
												var vendor = _vendors.Find(v => v.Id == vendorId);
												if (vendor == null)
												{
																vendor = await _vendorService.GetVendorById(vendorId);
												}
												if (vendor == null || user == null)
												{
																var errors = new List<string>();
																errors.Add("Vendor or User ID is invalid");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}

												var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
												if (GetUserRole() == "Admin" || vendor.AdminIds.Contains(userId))
												{
																try
																{
																				if (newMember.Role == RoleTypes.Admin)
																				{
																								vendor.AdminIds.Remove(newMember.UserId);
																								vendor.MemberIds.Add(newMember.UserId);
																								user.VendorIds.Remove(vendorId);
																				}
																				else
																				{
																								vendor.AdminIds.Remove(newMember.UserId);
																								vendor.MemberIds.Remove(newMember.UserId);
																				}
																				await _vendorService.UpdateVendor(vendor);
																				await _userService.UpdateUser(user.UserId, user);
																				return Ok();
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
												return Unauthorized();
								}

								//POST api/Vendors/5
								[HttpPost("add-location/{vendorId}")]
								public async Task<IActionResult> AddLocation(string vendorId, IDictionary<string, string> kvp)
								{
												var locationId = kvp["locationId"];
												var loc = await _locationService.Get(locationId);
												var vendor = _vendors.Find(v => v.Id == vendorId);
												if (vendor == null)
												{
																vendor = await _vendorService.GetVendorById(vendorId);
												}
												if (vendor == null || loc == null)
												{
																var errors = new List<string>();
																errors.Add("Vendor or Location ID is invalid");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}

												var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
												if (GetUserRole() == "Admin" || vendor.AdminIds.Contains(userId))
												{
																try
																{
																				vendor.LocationIds.Add(locationId);
																				loc.VendorId = vendorId;

																				await _vendorService.UpdateVendor(vendor);
																				await _locationService.Update(loc);
																				return Ok();
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
												return Unauthorized();
								}

								//POST api/Vendors/5
								[HttpPost("remove-location/{vendorId}")]
								public async Task<IActionResult> RemoveLocation(string vendorId, IDictionary<string, string> kvp)
								{
												var locationId = kvp["locationId"];
												var loc = await _locationService.Get(locationId);
												var vendor = _vendors.Find(v => v.Id == vendorId);
												if (vendor == null)
												{
																vendor = await _vendorService.GetVendorById(vendorId);
												}
												if (vendor == null || loc == null)
												{
																var errors = new List<string>();
																errors.Add("Vendor or Location ID is invalid");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}

												var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
												if (GetUserRole() == "Admin" || vendor.AdminIds.Contains(userId))
												{
																try
																{
																				vendor.LocationIds.Remove(locationId);
																				loc.VendorId = null;

																				await _vendorService.UpdateVendor(vendor);
																				await _locationService.Update(locationId, loc);
																				return Ok();
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
												return Unauthorized();
								}

								private string GetUserId(string action)
								{
												if (this.User.Claims.Any())
												{
																var id = this.User.Claims.Single(u => u.Type == "Id").Value;
																_identityService.Identify(id, SegmentIOKeys.Categories.Identity, action);
																return id;
												}
												return string.Empty;
								}

								private string GetUserRole()
								{
												if (this.User.Claims.Any())
												{
																return this.User.Claims.Single(u => u.Type == "Role").Value;
												}
												return string.Empty;
								}

								private async Task<PoppinUser> GetUserProfile(VendorMemberRequest req)
								{
												var user = await _userService.GetUserByEmail(req.Email);
												if (user == null)
												{
																var u = await _identityService.GetUserByEmail(req.Email);
																if (u != null)
																{
																				user = new PoppinUser(u.User);
																				_userService.AddUser(user);
																}
												}
												return user;
								}

								// DELETE api/Vendors/5
								[HttpDelete("{vendorId}")]
								public void Delete(string vendorId)
								{
								}
				}
}
