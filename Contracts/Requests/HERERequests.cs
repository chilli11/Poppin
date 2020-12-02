using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests.HERE
{
				public class FullGeocodeRequest
				{
								public string Q { get; set; }
								public StructuredGeocodeRequest QQ { get; set; }
				}

				public class StructuredGeocodeRequest
				{
								public string HouseNumber { get; set; }
								public string Street { get; set; }
								public string District { get; set; }
								public string City { get; set; }
								public string County { get; set; }
								public string State { get; set; }
								public string Country { get; set; }

								public override string ToString()
								{
												return this.AsStringDictionary().ToString();
								}
				}
}
