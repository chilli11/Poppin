using MongoDB.Driver.GeoJsonObjectModel;
using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Tracking
{
				public class SearchAction : LogAction
				{
								public string SearchTerm { get; set; }
								public string SearchLocation { get; set; }

								/// <summary>
								/// Comma delimited list
								/// </summary>
								public string SearchCategories { get; set; }
				}

				public class BasicLocationAction : LogAction
				{
								public string LocationId { get; set; }
				}

				public class GetDirectionsAction : LogAction
				{
								public string LocationId { get; set; }
								public GeoJsonPoint<GeoJson2DGeographicCoordinates> StartCoordinates { get; set; }
				}

				public class UpdateProfileAction : LogAction
				{
								public string FieldUpdated { get; set; }
								public string OldValue { get; set; }
								public string NewValue { get; set; }
				}

				public class UpdateGeoAction : LogAction
				{
								public GeoJsonPoint<GeoJson2DGeographicCoordinates> Coordinates { get; set; }
				}

				public class LogAction
				{

				}
}
