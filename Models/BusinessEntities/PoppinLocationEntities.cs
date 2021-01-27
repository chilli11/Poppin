using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver.GeoJsonObjectModel;
using Poppin.Contracts.Requests;
using Poppin.Extensions;
using Poppin.Models.Geocoding;
using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

namespace Poppin.Models.BusinessEntities
{

	public class PoppinSearchResponse
	{
		public int Total { get; set; }
		public int Offset { get; set; }
		public int PageLength { get; set; }
		public List<PoppinLocation> Businesses { get; set; }
		public LocationSearchRequest SearchParams { get; set; }
	}

	[BsonIgnoreExtraElements]
	public class Address
	{
		public Address() { }

		public Address(AddressDTO a)
		{
			Full = a.Full;
			Line1 = a.Line1;
			Line2 = a.Line2;
			City = a.City;

			if (StateTransforms.StateCodes.Contains(a.State))
			{
				State = a.State;
			}
			else if (StateTransforms.StatesWithCodes.ContainsKey(a.State))
			{
				State = StateTransforms.StatesWithCodes[a.State];
			}
			else
			{
				throw new Exception("Invalid `state`");
			}

			ZipCode = a.ZipCode;
			if (a.Geo != null)
			{
				var x = new GeoJson2DGeographicCoordinates(a.Geo.Coordinates[0], a.Geo.Coordinates[1]);
				Geo = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(x);
			}
			else if (a.Coordinates != null)
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
			Full = v.Address1 + ", " + City + ", " + State + " " + ZipCode;
			Geo = c.ToGeoJson();
		}

		public Address(Geocoding.HERE.Address a, Dictionary<string, double> pos)
		{
			Full = a.Full;
			Line1 = !string.IsNullOrEmpty(a.Line1) ? a.Line1 : a.Full;
			City = a.City;
			State = a.StateCode;
			ZipCode = a.ZipCode;

			var x = new GeoJson2DGeographicCoordinates(pos["lng"], pos["lat"]);
			Geo = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(x);
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
			Full = v.Address1 + ", " + City + ", " + State + " " + ZipCode;
			Coordinates = c;
		}

		public AddressDTO(Geocoding.HERE.Address a, Dictionary<string, double> pos)
		{
			Full = a.Full;
			Line1 = !string.IsNullOrEmpty(a.Line1) ? a.Line1 : a.Full;
			City = a.City;
			State = a.StateCode;
			ZipCode = a.ZipCode;

			Geo = new GeoCoords { Type = "Point", Coordinates = new[] { pos["lng"], pos["lat"] } };
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

	public class Menu
	{
		public string Name { get; set; }
		public string Url { get; set; }
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
		public PoppinLocation Location { get; set; }
		public YelpBusinessSearchResponse YelpMatches { get; set; }
	}
}
