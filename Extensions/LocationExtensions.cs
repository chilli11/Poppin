using Poppin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Extensions
{
				public static class LocationExtensions
				{
								public static void UpdateHours(this PoppinLocation loc, BusinessSchedule hours)
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
				}
}
