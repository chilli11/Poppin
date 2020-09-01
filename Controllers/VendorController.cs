using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Poppin.Contracts.Requests;
using Poppin.Contracts.Responses;
using Poppin.Interfaces;
using Poppin.Models;
using Poppin.Models.BusinessEntities;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Poppin.Controllers
{
				[Route("api/[controller]")]
				[ApiController]
				public class VendorController : ControllerBase
				{
								private readonly IVendorService _vendorService;
        private readonly ILocationService _locationService;
        private readonly IUserService _userService;

								private Vendor _vendor;

								public VendorController(IVendorService vendorService, ILocationService locationService, IUserService userService)
								{
												_vendorService = vendorService;
												_locationService = locationService;
												_userService = userService;
								}

								// GET: api/Vendor/5
								[HttpGet("{vendorId}")]
								public async Task<IActionResult> Get(string vendorId)
								{
												if (_vendor == null)
												{
																_vendor = await _vendorService.GetVendorById(vendorId);
												}
												if (_vendor == null)
												{
																var errors = new List<string>();
																errors.Add("Vendor ID is invalid");
																return BadRequest(new GenericFailure
																{
																				Errors = errors
																});
												}

												var vResult = new VendorResult
												{
																Vendor = _vendor,
																Locations = _vendor.GetLocations(_locationService),
																Admins = _vendor.GetAdmins(_userService),
																Members = _vendor.GetMembers(_userService)
												};
												return Ok(vResult);
								}

								// POST api/<VendorController>
								[HttpPost]
								public async Task<IActionResult> Post(PoppinVendorRequest newVendor)
								{
												var vendor = new Vendor(newVendor);
												var isExisting = await _vendorService.CheckExists(vendor);
												vendor.LastUpdate = DateTime.UtcNow;

												if (isExisting == null)
												{
																await _vendorService.AddVendor(vendor);
																_vendor = vendor;
																return CreatedAtAction("Post", vendor);
												}
												return Ok(isExisting);
								}

								// PUT api/<VendorController>/5
								[HttpPut]
								public async Task<IActionResult> Put(PoppinVendorRequest newVendor)
								{
												var vendor = new Vendor(newVendor);
												try
												{
																vendor.LastUpdate = DateTime.UtcNow;
																await _vendorService.UpdateVendor(vendor);
																_vendor = vendor;
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

								// PUT api/<VendorController>/5
								[HttpPut("{vendorId}")]
								public async Task<IActionResult> Put(string vendorId, PoppinVendorRequest newVendor)
								{
												var vendor = new Vendor(newVendor);
												try
												{
																vendor.LastUpdate = DateTime.UtcNow;
																await _vendorService.UpdateVendor(vendorId, vendor);
																_vendor = vendor;
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

								// DELETE api/<VendorController>/5
								[HttpDelete("{vendorId}")]
								public void Delete(string vendorId)
								{
								}
				}
}
