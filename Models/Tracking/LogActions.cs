using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Tracking
{
				public class Search : LogAction
				{
								public string SearchTerm { get; set; }
								public string SearchLocation { get; set; }

								/// <summary>
								/// Comma delimited list
								/// </summary>
								public string SearchCategories { get; set; }
				}

				public class ViewLocation : LogAction
				{
								public string LocationId { get; set; }
				}

				public class SaveLocation : LogAction
				{
								public string LocationId { get; set; }
				}

				public class HideLocation : LogAction
				{
								public string LocationId { get; set; }
				}

				public class Checkin : LogAction
				{
								public string LocationId { get; set; }
				}

				public class GetDirections : LogAction
				{
								public string LocationId { get; set; }
								public Coord StartCoordinates { get; set; }
				}

				public class UpdateProfile : LogAction
				{
								public string FieldUpdated { get; set; }
								public string OldValue { get; set; }
								public string NewValue { get; set; }
				}

				public class LogAction
				{

				}
}
