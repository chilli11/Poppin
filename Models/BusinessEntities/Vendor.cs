﻿using Poppin.Contracts.Requests;
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
								public IEnumerable<string> AdminIds { get; set; }
								public IEnumerable<string> MemberIds { get; set; }
								public IEnumerable<string> SubVendorIds { get; set; }
								public IEnumerable<string> LocationIds { get; set; }
								public string ParentVendorId { get; set; }
								public DateTime LastUpdate { get; set; }


								public IEnumerable<PoppinLocation> GetLocations(ILocationService locationsService)
								{
												if (LocationIds == null) return null;
												return locationsService.GetMany(LocationIds).Result;
								}

								public IEnumerable<PoppinUser> GetAdmins(IUserService userService)
								{
												if (AdminIds == null) return null;
												return userService.GetUsersById(AdminIds).Result;
								}

								public IEnumerable<PoppinUser> GetMembers(IUserService userService)
								{
												if (MemberIds == null) return null;
												return userService.GetUsersById(MemberIds).Result;
								}
				}
}
