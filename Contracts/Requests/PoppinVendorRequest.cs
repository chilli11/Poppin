using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
				public class PoppinVendorRequest
				{
								public PoppinVendorRequest() { }

								public string Id { get; set; }
								public string OrganizationName { get; set; }
								public string OrganizationContactName { get; set; }
								public string OrganizationContactEmail { get; set; }
								public bool Active { get; set; }
								public IEnumerable<string> LocationIds { get; set; }
								public IEnumerable<string> MemberIds { get; set; }
				}
}
