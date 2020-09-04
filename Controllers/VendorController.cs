using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Internal;
using Poppin.Contracts.Requests;
using Poppin.Contracts.Responses;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

								private List<Vendor> _vendors;

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

												var vResult = new VendorResult
												{
																Vendor = vendor,
																Locations = vendor.GetLocations(_locationService),
																Admins = vendor.GetAdmins(_userService),
																Members = vendor.GetMembers(_userService)
												};
												return Ok(vResult);
								}

								// GET: api/Vendor/5
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
																return Ok(vendors);
												}
								}

								// GET: api/Vendor/5
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
												_vendors.AddRange(leftovers);
												return Ok(vendors);
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
																_vendors.Add(vendor);
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

								// PUT api/<VendorController>/5
								[HttpPut("{vendorId}")]
								public async Task<IActionResult> Put(string vendorId, PoppinVendorRequest newVendor)
								{
												var vendor = new Vendor(newVendor);
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

								// DELETE api/<VendorController>/5
								[HttpDelete("{vendorId}")]
								public void Delete(string vendorId)
								{
								}
				}
}
