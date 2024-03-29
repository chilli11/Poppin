﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
				/// <summary>
				/// Request format to create a new <see cref="Models.BusinessEntities.Vendor"/>,
				/// or to edit an existing (if `Id` is included)
				/// </summary>
				public class PoppinVendorRequest
				{
								public PoppinVendorRequest() { }

								public string Id { get; set; }
								public string OrganizationName { get; set; }
								public string OrganizationContactName { get; set; }
								public string OrganizationContactEmail { get; set; }
								public bool Active { get; set; }
								public string[] LocationIds { get; set; }
								public string[] MemberIds { get; set; }
								public string[] SubVendorIds { get; set; }
								public string[] AdminIds { get; set; }
								public string ParentVendorId { get; set; }
				}
}
