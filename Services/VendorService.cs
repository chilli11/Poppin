using MongoDB.Driver;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Poppin.Services
{
				public class VendorService : IVendorService
				{
								private readonly IMongoCollection<Vendor> _vendors;

								public VendorService(IMongoDBSettings settings)
								{
												var client = new MongoClient(settings.ConnectionString);
												var database = client.GetDatabase(settings.DatabaseName);

												_vendors = database.GetCollection<Vendor>("Vendors");
								}
								
								public Task<Vendor> GetVendorById(string vendorId)
												=> _vendors.FindAsync(v => v.Id == vendorId).Result.FirstAsync();
								public Task<List<Vendor>> GetVendorsByIds(IEnumerable<string> vendorIds) =>
												_vendors.Find(v => vendorIds.Contains(v.Id)).ToListAsync();

								public Task<List<Vendor>> GetVendorsBySearch(string searchTerm)
								{
												bool isEmail = IsValidEmail(searchTerm);
												if (isEmail)
												{
																return _vendors.Find(v => v.OrganizationContactEmail == searchTerm).ToListAsync();
												}
												var output = _vendors.Find(v => v.OrganizationName == searchTerm).ToListAsync();
												if (output != null)
												{
																return output;
												}

												return _vendors.Find(v => v.OrganizationContactName == searchTerm).ToListAsync();
								}

								public Task AddVendor(Vendor vendor) => _vendors.InsertOneAsync(vendor);
								public Task UpdateVendor(Vendor vendor) => _vendors.ReplaceOneAsync(v => v.Id == vendor.Id, vendor);
								public Task UpdateVendor(string vendorId, Vendor vendor) => _vendors.ReplaceOneAsync(v => v.Id == vendorId, vendor);

								public Task<Vendor> CheckExists(Vendor vendor)
								{
												return _vendors.FindAsync(v => v.OrganizationName == vendor.OrganizationName)
																		.Result.FirstOrDefaultAsync();
								}
								private bool IsValidEmail(string email)
								{
												try
												{
																var addr = new System.Net.Mail.MailAddress(email);
																return addr.Address == email;
												}
												catch
												{
																return false;
												}
								}
				}
}
