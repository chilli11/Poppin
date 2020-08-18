using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
								private readonly IIdentityService _identityService;

								public Vendor (ILocationService locationService, IIdentityService identityService)
								{
												_locationsService = locationService;
												_identityService = identityService;
								}

								[BsonId]
								[BsonRepresentation(BsonType.ObjectId)]
								public string Id { get; set; }
								public string OrganizationName { get; set; }
								public string OrganizationContactName { get; set; }
								public string OrganizationContactEmail { get; set; }
								public bool Active { get; set; }
								public IEnumerable<string> LocationIds { get; set; }
								public IEnumerable<string> MemberIds { get; set; }

								public virtual IEnumerable<PoppinLocation> Locations
								{
												get
												{
																return _locationsService.GetMany(LocationIds).Result;
												}
								}

								public virtual IEnumerable<User> Members
								{
												get
												{
																return _identityService.GetUsersById(MemberIds).Result.Users;
												}
								}
				}
}
