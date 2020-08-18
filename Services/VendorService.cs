using MongoDB.Driver;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models;
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
								{
												return _vendors.FindAsync(v => v.Id == vendorId).Result.FirstAsync();
								}
								//public Task UpdateVendor(Vendor vendor);
								//public Task UpdateVendor(string vendorId, Vendor vendor);
				}
}
