using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface IVendorService
				{
								public Task<Vendor> GetVendorById(string vendorId);
								public Task<List<Vendor>> GetVendorsByIds(IEnumerable<string> vendorIds);
								public Task<List<Vendor>> GetVendorsBySearch(string searchTerm);
								public Task AddVendor(Vendor vendor);
								public Task UpdateVendor(Vendor vendor);
								public Task UpdateVendor(string vendorId, Vendor vendor);
								public Task<Vendor> CheckExists(Vendor vendor);
				}
}
