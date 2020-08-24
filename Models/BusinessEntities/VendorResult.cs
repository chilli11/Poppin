using Microsoft.AspNetCore.Identity;
using Poppin.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.BusinessEntities
{
				public class VendorResult
				{
								public Vendor Vendor { get; set; }
								public IEnumerable<PoppinLocation> Locations { get; set; }
								public IEnumerable<PoppinUser> Members { get; set; }
				}
}
