using MongoDB.Driver.GeoJsonObjectModel;
using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models
{

				public class PoppinSearchResponse
				{
								public int Total { get; set; }
								public List<PoppinLocation> Businesses { get; set; }
								public YelpRegion Region { get; set; }
								public IYelpSearchParams SearchParams { get; set; }
				}

				public class Address
				{
								public Address() { }

								public Address(YelpLocation v, Coord c)
								{
												Line1 = v.Address1;
												Line2 = v.Address2;
												City = v.City;
												State = v.State;

												ZipCode = int.Parse(v.ZipCode);
												Geo = c.ToGeoJson();
								}

								public string Line1 { get; set; }
								public string Line2 { get; set; }
								public string City { get; set; }
								public string State { get; set; }
								public int? ZipCode { get; set; }
								public GeoJsonPoint<GeoJson2DGeographicCoordinates> Geo { get; set; }
				}

				public class PoppinInsertActionResponse
				{
								public PoppinLocation location { get; set; }
								public YelpBusinessSearchResponse yelpMatches { get; set; }
				}
}
