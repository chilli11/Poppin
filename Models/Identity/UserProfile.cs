using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity
{
				public class UserProfile : ProfileEntity
    {
        public IEnumerable<string> VendorIds { get; set; }

        public virtual IEnumerable<Vendor> Vendors
        {
            get
            {
                return new List<Vendor>();
            }
        }
        public IEnumerable<PoppinLocation> Affiliations
        {
            get
            {
                return Vendors.SelectMany(v => v.Locations);
            }
        }
    }
}
