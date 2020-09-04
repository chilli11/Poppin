using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using System;
using System.Collections.Generic;

namespace Poppin.Models.BusinessEntities
{
				public class Vendor
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

								[BsonId]
								[BsonRepresentation(BsonType.ObjectId)]
								public string Id { get; set; }
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
												return locationsService.GetMany(LocationIds).Result;
								}

								public IEnumerable<PoppinUser> GetAdmins(IUserService userService)
								{
												return userService.GetUsersById(AdminIds).Result;
								}

								public IEnumerable<PoppinUser> GetMembers(IUserService userService)
								{
												return userService.GetUsersById(MemberIds).Result;
								}

								public IEnumerable<Vendor> GetSubVendors(IVendorService vendorService)
								{
												return vendorService.GetVendorsByIds(SubVendorIds).Result;
								}
				}
}
