using Microsoft.AspNetCore.Identity;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity
{
				public class PoppinUser : ProfileEntity
    {
        public PoppinUser(IdentityUser user)
								{
            UserId = user.Id;
            Username = user.UserName;
								}

        public IEnumerable<string> VendorIds { get; set; }

        public async Task<List<Vendor>> GetVendors(IVendorService vs)
								{
            return await vs.GetVendorsByIds(VendorIds);
								}
    }
}
