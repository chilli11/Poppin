using Poppin.Models;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Extensions
{
				public static class LocationExtensions
				{
								public static void UpdateHours(this PoppinLocation loc, List<HourSet> hours)
								{
												loc.Hours = hours;
								}

								public static void IncrementCrowdSize(this PoppinLocation loc)
								{
												loc.CrowdSize += 1;
								}

								public static void DecrementCrowdSize(this PoppinLocation loc)
								{
												if (loc.CrowdSize > 0)
												{
																loc.CrowdSize -= 1;
												}
								}

								public static void ChangeCrowdSize(this PoppinLocation loc, int change)
								{
												loc.CrowdSize += change;
												if (loc.CrowdSize < 0)
												{
																loc.CrowdSize = 0;
												}
								}

								public static void UpdateCrowdSizes(this List<PoppinLocation> locs, IEnumerable<Checkin> checkins)
								{
												locs.ForEach(l => l.SetCrowdSize(checkins.Where(c => c.LocationId == l.Id)));
								}
				}
}
