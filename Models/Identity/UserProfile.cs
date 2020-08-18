using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity
{
				public class UserProfile : ProfileEntity
    {
        public IEnumerable<string> VendorIds { get; set; }

        public async Task<List<Vendor>> GetVendors(IVendorService vs)
								{
            return await vs.GetVendorsByIds(VendorIds);
								}
    }
}
