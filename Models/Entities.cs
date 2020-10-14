using MongoDB.Driver.GeoJsonObjectModel;
using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models
{
				public class HourSet
				{
								private List<string> days = new List<string>() {
												"Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday"
								};

								public HourSet() { }

								public HourSet(YelpHourEntity y)
								{
												Opening = y.Start.Substring(0,2) + ':' + y.Start.Substring(2);
												Closing = y.End.Substring(0, 2) + ':' + y.End.Substring(2);
												Day = days[y.Day];
								}

								public string Opening { get; set; }
								public string Closing { get; set; }
								public string Day { get; set; }
				}

				public class Category
				{
								public string Slug { get; set; }
								public string Name { get; set; }
								public string Parent { get; set; }
								public HashSet<string> Related { get; set; }
								public HashSet<string> Children { get; set; }
				}

				public class Coord
				{
								public Coord() { }
								public Coord(CoordDTO v)
								{
												if (!string.IsNullOrEmpty(v.Latitude))
												{
																Latitude = Single.Parse(v.Latitude);
												}
												if (!string.IsNullOrEmpty(v.Longitude))
												{
																Longitude = Single.Parse(v.Longitude);
												}
								}

								public double Latitude { get; set; }
								public double Longitude { get; set; }

								public GeoJsonPoint<GeoJson2DGeographicCoordinates> ToGeoJson()
								{
												var lat = Convert.ToDouble(Latitude);
												var lon = Convert.ToDouble(Longitude);
												return new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(lon, lat));
								}
				}

				public class CoordDTO
				{
								public string Latitude { get; set; }
								public string Longitude { get; set; }

								public GeoJsonPoint<GeoJson2DGeographicCoordinates> ToGeoJson()
								{
												var lat = Convert.ToDouble(Latitude);
												var lon = Convert.ToDouble(Longitude);
												return new GeoJsonPoint<GeoJson2DGeographicCoordinates>(new GeoJson2DGeographicCoordinates(lon, lat));
								}
				}


}
