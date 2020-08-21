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
												var vendor = await _vendorService.GetVendorById(vendorId);
												if (vendor == null)
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
																Vendor = vendor,
																Locations = vendor.GetLocations(_locationService),
																Members = vendor.GetMembers(_userService)
												};
												return Ok(vResult);
								}

								// POST api/<VendorController>
								[HttpPost]
								public async Task<IActionResult> Post(PoppinVendorRequest _vendor)
								{
												var vendor = new Vendor(_vendor);
												var isExisting = await _vendorService.CheckExists(vendor);
												vendor.LastUpdate = DateTime.Now;

												if (isExisting == null)
												{
																await _vendorService.AddVendor(vendor);
																return CreatedAtAction("Post", vendor);
												}
												return Ok(isExisting);
								}

								// PUT api/<VendorController>/5
								[HttpPut]
								public async Task<IActionResult> Put(PoppinVendorRequest _vendor)
								{
												var vendor = new Vendor(_vendor);
												try
												{
																vendor.LastUpdate = DateTime.Now;
																await _vendorService.UpdateVendor(vendor);
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
								public async Task<IActionResult> Put(string vendorId, PoppinVendorRequest _vendor)
								{
												var vendor = new Vendor(_vendor);
												try
												{
																vendor.LastUpdate = DateTime.Now;
																await _vendorService.UpdateVendor(vendorId, vendor);
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
