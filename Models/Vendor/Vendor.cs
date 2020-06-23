using Poppin.Interfaces;
using Poppin.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models
{
				public class Vendor : ProfileEntity
				{
								private readonly ILocationService _locationsService;

								public Vendor (ILocationService locationService)
								{
												_locationsService = locationService;
								}

								public string OrganizationName { get; set; }
								public string OrganizationContactName { get; set; }
								public string OrganizationContactEmail { get; set; }
								public bool Active { get; set; }
								public IEnumerable<string> LocationIds { get; set; }
								public IEnumerable<string> MemberIds { get; set; }

								public IEnumerable<PoppinLocation> Affiliations
								{
												get
												{
																return _locationsService.GetMany(LocationIds).Result;
												}
								}
				}
}
