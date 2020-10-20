using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using Poppin.Extensions;
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

				[BsonIgnoreExtraElements]
				public class Address
				{
								public Address() { }

								public Address(AddressDTO a)
								{
												Line1 = a.Line1;
												Line2 = a.Line2;
												City = a.City;
												State = a.State;

												ZipCode = a.ZipCode;
												if (a.Geo != null)
												{
																var x = new GeoJson2DGeographicCoordinates(a.Geo.Coordinates[0], a.Geo.Coordinates[1]);
																Geo = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(x);
												}
												else
												{
																Geo = a.Coordinates.ToGeoJson();
												}
								}

								public Address(YelpLocation v, Coord c)
								{
												Line1 = v.Address1;
												Line2 = v.Address2;
												City = v.City;
												State = v.State;

												ZipCode = v.ZipCode;
												Geo = c.ToGeoJson();
								}

								public string Full { get; set; }
								public string Line1 { get; set; }
								public string Line2 { get; set; }
								public string City { get; set; }
								public string State { get; set; }

								[BsonSerializer(typeof(TestingObjectTypeSerializer))]
								public string ZipCode { get; set; }
								public Coord Coordinates {
												get
												{
																return new Coord()
																{
																				Latitude = Geo.Coordinates.Latitude,
																				Longitude = Geo.Coordinates.Longitude
																};
												}
								}
								public GeoJsonPoint<GeoJson2DGeographicCoordinates> Geo { get; set; }
				}

				[BsonIgnoreExtraElements]
				public class AddressDTO
				{
								public AddressDTO() { }

								public AddressDTO(YelpLocation v, Coord c)
								{
												Line1 = v.Address1;
												Line2 = v.Address2;
												City = v.City;
												State = v.State;

												ZipCode = v.ZipCode;
												Coordinates = c;
								}

								public string Full { get; set; }
								public string Line1 { get; set; }
								public string Line2 { get; set; }
								public string City { get; set; }
								public string State { get; set; }
								public string ZipCode { get; set; }
								public Coord Coordinates { get; set; }
								public GeoCoords Geo { get; set; }
				}

				/// <summary>
				/// Uses GeoJson Point geometry type 
				/// https://developer.here.com/documentation/studio/map_customization_suite_hlp/dev_guide/topics/geojson-basics.html
				/// https://geojsonlint.com/
				/// </summary>
				public class GeoCoords
				{
								public string Type { get; set; }
								public double[] Coordinates { get; set; }
				}

				public class PoppinInsertActionResponse
				{
								public PoppinLocation location { get; set; }
								public YelpBusinessSearchResponse yelpMatches { get; set; }
				}
}
