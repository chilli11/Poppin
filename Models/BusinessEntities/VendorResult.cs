using System.Collections.Generic;

namespace Poppin.Models.BusinessEntities
{
				public class VendorResult
				{
								public Vendor Vendor { get; set; }
								public IEnumerable<PoppinUser> Admins { get; set; }
								public IEnumerable<PoppinLocation> Locations { get; set; }
								public IEnumerable<PoppinUser> Members { get; set; }
				}
}
