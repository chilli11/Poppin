﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Poppin.Contracts.Requests;
using Poppin.Contracts.Responses;
using Poppin.Extensions;
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
	public class VendorsController : PoppinBaseController
	{
		public VendorsController(
			IVendorService vendorService,
			ILocationService locationService,
			IUserService userService,
			IIdentityService identityService,
			IBestTimeService btService,
			IBigDataCloudService bdcService)
		{
			_vendorService = vendorService;
			_locationService = locationService;
			_userService = userService;
			_identityService = identityService;
			_btService = btService;
			_bdcService = bdcService;
		}

		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
			var profile = await GetUserProfile(userId);
			var vendors = await _vendorService.GetVendorsByIds(profile.VendorIds);
			if (vendors == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "No Results Found" }
				});
			}
			return Ok(vendors);
		}

		// GET: api/Vendors/5
		[Authorize(Policy = RoleTypes.Admin)]
		[HttpGet("all")]
		public async Task<IActionResult> GetAll()
		{
			var vendors = await _vendorService.GetAll();
			if (vendors == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "No Results Found" }
				});
			}
			return Ok(vendors);
		}

		// GET: api/Vendors/5
		[HttpGet("{vendorId}")]
		public async Task<IActionResult> Get(string vendorId)
		{			
			var vendor = await _vendorService.GetVendorById(vendorId);
			if (vendor == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "Vendor ID is invalid" }
				});
			}

			var userId = GetUserId(SegmentIOKeys.Actions.ViewVendor);
			if (vendor.AdminIds.Contains(userId) || vendor.MemberIds.Contains(userId) || GetUserRole() == RoleTypes.Admin)
			{
				var locations = vendor.GetLocations(_locationService).ToList();
				locations.StoreForecasts(_btService);
				locations.ForEach((l) => {
					if (l.TimeZone == null || l.TimeZone.UtcOffset == null)
					{
						l = _bdcService.GetTimeZoneInfo(l).Result;
						_locationService.Update(l);
					}
				});


				var vResult = new VendorResult
				{
					Vendor = vendor,
					SubVendors = vendor.GetSubVendors(_vendorService),
					Locations = locations,
					Admins = vendor.GetAdmins(_userService),
					Members = vendor.GetMembers(_userService)
				};
				if (vResult.Locations != null)
				{
					var checkins = await _locationService.GetCheckinsForLocations(vResult.Locations.Select(l => l.Id));
					vResult.Locations.ToList().UpdateCrowdSizes(checkins);
				}
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
			var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
			var vendors = await _vendorService.GetVendorsByIds(vendorIds);

			if (vendors == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "No Results Found" }
				});
			}

			if (GetUserRole() == RoleTypes.Admin)
			{
				return Ok(vendors);
			}
			return Ok(vendors.Where(v => v.AdminIds.Contains(userId) || v.MemberIds.Contains(userId)));
		}

		// GET: api/Vendors/5
		[HttpPost("get-by-search")]
		public async Task<IActionResult> GetBySearch(string searchTerm)
		{
			var vendors = await _vendorService.GetVendorsBySearch(searchTerm);
			if (vendors == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "No Results Found" }
				});
			}
			var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);

			if (GetUserRole() == RoleTypes.Admin)
			{
				return Ok(vendors);
			}
			return Ok(vendors.Where(v => v.AdminIds.Contains(userId) || v.MemberIds.Contains(userId)));
		}

		// POST api/Vendors
		[Authorize(Policy = RoleTypes.Admin)]
		[HttpPost]
		public async Task<IActionResult> Post(PoppinVendorRequest newVendor)
		{
			var vendor = new Vendor(newVendor);
			var isExisting = await _vendorService.CheckExists(vendor);
			vendor.LastUpdate = DateTime.UtcNow;

			if (isExisting == null)
			{
				await _vendorService.AddVendor(vendor);
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
			if (vendor.AdminIds.Contains(userId) || GetUserRole() == RoleTypes.Admin)
			{
				try
				{
					vendor.LastUpdate = DateTime.UtcNow;
					await _vendorService.UpdateVendor(vendor);
					return Ok(vendor);
				}
				catch (Exception e)
				{
					return BadRequest(new GenericFailure
					{
						Errors = new[] { e.Message }
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
			if (vendor.AdminIds.Contains(userId) || GetUserRole() == RoleTypes.Admin)
			{
				try
				{
					vendor.LastUpdate = DateTime.UtcNow;
					await _vendorService.UpdateVendor(vendorId, vendor);
					return Ok(vendor);
				}
				catch (Exception e)
				{
					return BadRequest(new GenericFailure
					{
						Errors = new[] { e.Message }
					});
				}
			}
			return Unauthorized();
		}

		//POST api/Vendors/5
		[HttpPost("add-member/{vendorId}")]
		public async Task<IActionResult> AddMember(string vendorId, VendorMemberRequest newMember)
		{
			var user = await GetUserProfileByEmail(newMember.Email);
			var vendor = await _vendorService.GetVendorById(vendorId);
			if (vendor == null || user == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "Vendor or User ID is invalid" }
				});
			}

			var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
			if (vendor.AdminIds.Contains(userId) || GetUserRole() == RoleTypes.Admin)
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
					return Ok(new VendorResult
					{
						Vendor = vendor,
						Members = vendor.GetMembers(_userService),
						Admins = vendor.GetAdmins(_userService)
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
			return Unauthorized();
		}

		//POST api/Vendors/5
		[HttpPost("remove-member/{vendorId}")]
		public async Task<IActionResult> RemoveMember(string vendorId, VendorMemberRequest newMember)
		{
			var user = await GetUserProfileByEmail(newMember.Email);
			var vendor = await _vendorService.GetVendorById(vendorId);
			if (vendor == null || user == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "Vendor or User ID is invalid" }
				});
			}

			var authRole = GetUserRole();
			var userId = GetUserId(SegmentIOKeys.Actions.ViewVendorList);
			if (vendor.AdminIds.Contains(userId) || authRole == RoleTypes.Admin)
			{
				try
				{
					if (newMember.Role == RoleTypes.Admin)
					{
						if (userId == newMember.UserId && authRole != RoleTypes.Admin)
						{
							return BadRequest(new GenericFailure
							{
								Errors = new[] { "You can't remove your own Admin privileges!" }
							});
						}
						vendor.AdminIds.Remove(newMember.UserId);
						vendor.MemberIds.Add(newMember.UserId);
					}
					else
					{
						vendor.AdminIds.Remove(newMember.UserId);
						vendor.MemberIds.Remove(newMember.UserId);
						user.VendorIds.Remove(vendorId);
					}
					await _vendorService.UpdateVendor(vendor);
					await _userService.UpdateUser(user.UserId, user);
					return Ok(new VendorResult
					{
						Vendor = vendor,
						Members = vendor.GetMembers(_userService),
						Admins = vendor.GetAdmins(_userService)
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
			return Unauthorized();
		}

		//POST api/Vendors/add-location/5
		/// <summary>
		/// Adds location ID to `Vendor.LocationIds`
		/// NEEDS: there's a null ref error when trying to update `PoppinLocation.VendorId`
		/// from the Mongo service
		/// </summary>
		/// <param name="vendorId"></param>
		/// <param name="kvp"></param>
		/// <returns></returns>
		[Authorize(Policy = RoleTypes.Admin)]
		[HttpPost("add-location/{vendorId}")]
		public async Task<IActionResult> AddLocation(string vendorId, IDictionary<string, string> kvp)
		{
			var locationId = kvp["locationId"];
			var loc = await _locationService.Get(locationId);
			var vendor = await _vendorService.GetVendorById(vendorId);
			if (vendor == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "Vendor ID is invalid." }
				});
			}
			if (loc == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "Locataion ID is invalid." }
				});
			}

			if (loc.VendorId != null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "Location has already been claimed." }
				});
			}

			GetUserId(SegmentIOKeys.Actions.ViewVendorList);
			try
			{
				vendor.LocationIds.Add(locationId);
				loc.VendorId = vendorId;

				await _vendorService.UpdateVendor(vendor);
				await _locationService.Update(loc);
				return Ok(new VendorResult
				{
					Vendor = vendor,
					Locations = vendor.GetLocations(_locationService)
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

		//POST api/Vendors/remove-location/5
		/// <summary>
		/// Removes location ID from `Vendor.LocationIds`
		/// NEEDS: there's a null ref error when trying to update `PoppinLocation.VendorId`
		/// from the Mongo service
		/// </summary>
		/// <param name="vendorId"></param>
		/// <param name="kvp"></param>
		/// <returns></returns>
		[Authorize(Policy = RoleTypes.Admin)]
		[HttpPost("remove-location/{vendorId}")]
		public async Task<IActionResult> RemoveLocation(string vendorId, IDictionary<string, string> kvp)
		{
			var vendor = await _vendorService.GetVendorById(vendorId);
			if (vendor == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "Vendor ID is invalid." }
				});
			}

			var locationId = kvp["locationId"];
			var loc = await _locationService.Get(locationId);
			if (loc == null)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "Location ID is invalid." }
				});
			}
			if (loc.VendorId != null && loc.VendorId != vendorId)
			{
				return BadRequest(new GenericFailure
				{
					Errors = new[] { "Location not claimed by this vendor." }
				});
			}

			GetUserId(SegmentIOKeys.Actions.ViewVendorList);
			try
			{
				vendor.LocationIds.Remove(locationId);
				loc.VendorId = null;

				await _vendorService.UpdateVendor(vendor);
				await _locationService.Update(locationId, loc);
				return Ok(new VendorResult
				{
					Vendor = vendor,
					Locations = vendor.GetLocations(_locationService)
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
	}
}
