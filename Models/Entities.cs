using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models
{
				public class BusinessSchedule
				{
								public BusinessSchedule() { }
								public BusinessSchedule(BusinessScheduleDTO v)
								{
												Sunday = new Day(v.Sunday);
												Monday = new Day(v.Monday);
												Tuesday = new Day(v.Tuesday);
												Wednesday = new Day(v.Wednesday);
												Thursday = new Day(v.Thursday);
												Friday = new Day(v.Friday);
												Saturday = new Day(v.Saturday);
								}

								public BusinessSchedule(YelpHourSet y)
								{
												Sunday = new Day(y.Open.FirstOrDefault(h => h.Day == 0));
												Monday = new Day(y.Open.FirstOrDefault(h => h.Day == 1));
												Tuesday = new Day(y.Open.FirstOrDefault(h => h.Day == 2));
												Wednesday = new Day(y.Open.FirstOrDefault(h => h.Day == 3));
												Thursday = new Day(y.Open.FirstOrDefault(h => h.Day == 4));
												Friday = new Day(y.Open.FirstOrDefault(h => h.Day == 5));
												Saturday = new Day(y.Open.FirstOrDefault(h => h.Day == 6));
								}

								public Day Sunday { get; set; }
								public Day Monday { get; set; }
								public Day Tuesday { get; set; }
								public Day Wednesday { get; set; }
								public Day Thursday { get; set; }
								public Day Friday { get; set; }
								public Day Saturday { get; set; }
				}
				public class BusinessScheduleDTO
				{
								public DayDTO Sunday { get; set; }
								public DayDTO Monday { get; set; }
								public DayDTO Tuesday { get; set; }
								public DayDTO Wednesday { get; set; }
								public DayDTO Thursday { get; set; }
								public DayDTO Friday { get; set; }
								public DayDTO Saturday { get; set; }
				}

				public class Day
				{
								public Day() { }
								public Day(DayDTO v)
								{
												if (!string.IsNullOrEmpty(v.OpeningHour))
												{
																Opening = Int32.Parse(v.OpeningHour);
																if (!string.IsNullOrEmpty(v.OpeningMinute))
																{
																				Opening += Int32.Parse(v.OpeningMinute);
																}
												}
												if (!string.IsNullOrEmpty(v.ClosingHour))
												{
																Closing = Int32.Parse(v.ClosingHour);
																if (!string.IsNullOrEmpty(v.ClosingMinute))
																{
																				Closing += Int32.Parse(v.ClosingMinute);
																}
												}
								}

								public Day(YelpHourEntity y)
								{
												Opening = int.Parse(y.Start);
												Closing = int.Parse(y.End);
								}

								public int Opening { get; set; }
								public int Closing { get; set; }
				}

				public class DayDTO
				{
								public string OpeningHour { get; set; }
								public string OpeningMinute { get; set; }
								public string ClosingHour { get; set; }
								public string ClosingMinute { get; set; }
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
				}

				public class CoordDTO
				{
								public string Latitude { get; set; }
								public string Longitude { get; set; }
				}
}
