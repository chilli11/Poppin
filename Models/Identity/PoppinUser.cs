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

        /// <summary>
        /// LocationIds saved by user
        /// </summary>
        public IEnumerable<string> Favorites { get; set; }

        /// <summary>
        /// LocationIds hidden by user
        /// </summary>
        public IEnumerable<string> Hidden { get; set; }

        /// <summary>
        /// Vendors that the user is a member of
        /// </summary>
        public IEnumerable<string> VendorIds { get; set; }

        public async Task<List<Vendor>> GetVendors(IVendorService vs)
								{
            return await vs.GetVendorsByIds(VendorIds);
								}
    }
}
