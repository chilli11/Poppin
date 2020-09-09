using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

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
												LocationIds = v.LocationIds.ToList();
												MemberIds = v.MemberIds.ToList();
												SubVendorIds = v.SubVendorIds.ToList();
												AdminIds = v.AdminIds.ToList();
												ParentVendorId = v.ParentVendorId;
								}

								public Vendor() { }

								[BsonId]
								[BsonRepresentation(BsonType.ObjectId)]
								public string Id { get; set; }
								public string OrganizationName { get; set; }
								public string OrganizationContactName { get; set; }
								public string OrganizationContactEmail { get; set; }
								public bool Active { get; set; }
								public List<string> AdminIds { get; set; }
								public List<string> MemberIds { get; set; }
								public List<string> SubVendorIds { get; set; }
								public List<string> LocationIds { get; set; }
								public string ParentVendorId { get; set; }
								public DateTime LastUpdate { get; set; }


								public IEnumerable<PoppinLocation> GetLocations(ILocationService locationsService)
								{
												if (LocationIds == null || LocationIds.Count == 0) return null;
												return locationsService.GetMany(LocationIds).Result;
								}

								public IEnumerable<PoppinUser> GetAdmins(IUserService userService)
								{
												if (AdminIds == null || AdminIds.Count == 0) return null;
												return userService.GetUsersById(AdminIds).Result;
								}

								public IEnumerable<PoppinUser> GetMembers(IUserService userService)
								{
												if (MemberIds == null || MemberIds.Count == 0) return null;
												return userService.GetUsersById(MemberIds).Result;
								}

								public IEnumerable<Vendor> GetSubVendors(IVendorService vendorService)
								{
												if (SubVendorIds == null || SubVendorIds.Count == 0) return null;
												return vendorService.GetVendorsByIds(SubVendorIds).Result;
								}
				}
}
