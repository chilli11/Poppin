using Microsoft.AspNetCore.Identity;
using Poppin.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.BusinessEntities
{
				public class PoppinUserResult
				{
								public PoppinUser User { get; set; }
								public IEnumerable<Vendor> Vendors { get; set; }
								public IEnumerable<PoppinLocation> Favorites { get; set; }
								public IEnumerable<PoppinLocation> Hidden { get; set; }
				}
}
