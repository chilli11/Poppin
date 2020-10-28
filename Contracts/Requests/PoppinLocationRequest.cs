using Poppin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
				/// <summary>
				/// Request format to create a new <see cref="PoppinLocation"/>,
				/// or to edit an existing (if `Id` is included)
				/// </summary>
				public class PoppinLocationRequest
				{
								public PoppinLocationRequest() { }

								public string Id { get; set; }
								public string YelpId { get; set; }
								public string VendorId { get; set; }
								public string Name { get; set; }
								public string Phone { get; set; }
								public AddressDTO Address { get; set; }
								public string LogoUrl { get; set; }
								public string MainPhotoUrl { get; set; }
								public string[] AddlPhotoUrls { get; set; }
								public string Website { get; set; }
								public HashSet<Menu> Menus { get; set; }
								public string YelpUrl { get; set; }
								public string[] Categories { get; set; }
								public int Capacity { get; set; }
								public int CrowdSize { get; set; }
								public int VisitLength { get; set; }
								public IList<HourSet> Hours { get; set; }
				}
}
