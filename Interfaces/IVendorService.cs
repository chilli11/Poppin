using Poppin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface IVendorService
				{
								public Task<Vendor> GetVendorById(string vendorId);
								//public Task UpdateVendor(Vendor vendor);
								//public Task UpdateVendor(string vendorId, Vendor vendor);
				}
}
