using Poppin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
				public class PoppinLocationRequest
				{
								public PoppinLocationRequest() { }

								public string Id { get; set; }
								public string YelpId { get; set; }
								public string Name { get; set; }
								public string Phone { get; set; }
								public Address Address { get; set; }
								public string[] Categories { get; set; }
								public int Capacity { get; set; }
								public int CrowdSize { get; set; }
								public IList<HourSet> Hours { get; set; }
				}
}
