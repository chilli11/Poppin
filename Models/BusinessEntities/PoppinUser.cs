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
            Email = user.Email;
								}

								public string ProfilePhoto { get; set; }
								public int Age { get; set; }
								public Gender Gender { get; set; }

								/// <summary>
								/// LocationIds saved by user
								/// </summary>
								public HashSet<string> Favorites { get; set; }

        /// <summary>
        /// LocationIds hidden by user
        /// </summary>
        public HashSet<string> Hidden { get; set; }

        /// <summary>
        /// Vendors that the user is a member of
        /// </summary>
        public HashSet<string> VendorIds { get; set; }

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

    public enum Gender
				{
        Male,
        Female
				}
}
