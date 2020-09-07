using Microsoft.AspNetCore.Identity;
using Poppin.Interfaces;
using Poppin.Models.Identity;
using Poppin.Models.Tracking;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poppin.Models.BusinessEntities
{
				public class PoppinUser : ProfileEntity
    {
        public PoppinUser(User user)
								{
            UserId = user.Id;
            Username = user.UserName;
            Role = user.Role;
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
            if (VendorIds == null) return null;
            return await vs.GetVendorsByIds(VendorIds);
        }
        public async Task<List<PoppinLocation>> GetFavorites(ILocationService ls)
        {
            if (Favorites == null) return null;
            return await ls.GetMany(Favorites);
        }
        public async Task<List<PoppinLocation>> GetHidden(ILocationService ls)
        {
            if (Hidden == null) return null;
            return await ls.GetMany(Hidden);
        }
        public async Task<IEnumerable<UserLog>> GetuserActivity(ILogActionService las)
        {
            return await las.GetUserActivity(Id);
        }
    }
}
