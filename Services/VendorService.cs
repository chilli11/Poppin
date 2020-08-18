using MongoDB.Driver;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
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
								public Task<List<Vendor>> GetVendorsByIds(IEnumerable<string> vendorIds)
												=> _vendors.FindAsync(v => vendorIds.Contains(v.Id)).Result.ToListAsync();

								public Task AddVendor(Vendor vendor) => _vendors.InsertOneAsync(vendor);
								public Task UpdateVendor(Vendor vendor) => _vendors.ReplaceOneAsync(v => v.Id == vendor.Id, vendor);
								public Task UpdateVendor(string vendorId, Vendor vendor) => _vendors.ReplaceOneAsync(v => v.Id == vendorId, vendor);

								public Task<Vendor> CheckExists(Vendor vendor)
								{
												return _vendors.FindAsync(v => v.OrganizationName == vendor.OrganizationName)
																		.Result.FirstOrDefaultAsync();
								}
				}
}
