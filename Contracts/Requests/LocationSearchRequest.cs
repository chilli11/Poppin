using Poppin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
				public class LocationSearchRequest
				{
								public string Term { get; set; }
								public GeoCoords Geo { get; set; }
								public double Radius { get; set; }
								public HashSet<string> Categories { get; set; }
				}
}
