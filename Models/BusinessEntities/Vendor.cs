using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using System;
using System.Collections.Generic;

namespace Poppin.Models.BusinessEntities
{
				public class Vendor : ProfileEntity
				{
								public Vendor(PoppinVendorRequest v)
								{
												Id = v.Id;
												OrganizationName = v.OrganizationName;
												OrganizationContactName = v.OrganizationContactName;
												OrganizationContactEmail = v.OrganizationContactEmail;
												Active = v.Active;
												LocationIds = v.LocationIds;
												MemberIds = v.MemberIds;
								}

								public Vendor() { }

								public string OrganizationName { get; set; }
								public string OrganizationContactName { get; set; }
								public string OrganizationContactEmail { get; set; }
								public bool Active { get; set; }
								public IEnumerable<string> LocationIds { get; set; }
								public IEnumerable<string> MemberIds { get; set; }
								public DateTime LastUpdate { get; set; }


								public IEnumerable<PoppinLocation> GetLocations(ILocationService locationsService)
								{
												return locationsService.GetMany(LocationIds).Result;
								}

								public IEnumerable<PoppinUser> GetMembers(IUserService userService)
								{
												return userService.GetUsersById(MemberIds).Result;
								}
				}
}
