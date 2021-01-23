using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.BestTimeEntities
{
	public class BestTimeWeek : IForecast
	{
		public BTWAnalysis Analysis { get; set; }
		public int EpochAnalysis { get; set; }
		public string VenueName { get; set; }
		public BTWWindow Window { get; set; }
		public DateTime ForecastUpdatedOn { get; set; }
		public BTVenueInfo VenueInfo { get; set; }

		public double GetForecastOccupancy()
		{
			var now = DateTime.Now;
			var day = FixDay(now.Hour, (int)now.DayOfWeek);
			var hour = FixHour(now.Hour);
			return Analysis.WeekRaw[day].DayRaw[hour] / 100;
		}

		private int FixDay(int hour, int day)
		{
			if (day == 0) day = 6;
			else day--;

			if (hour < 6)
			{
				if (day == 0) day = 6;
				else day--;
			}
			return day;
		}

		private int FixHour(int hour)
		{
			if (hour < 6) return hour + 18;
			return hour - 6;
		}
	}

	public class BTWAnalysis
	{
		public BTWDayRaw[] WeekRaw { get; set; }
	}

	public class BTWDayRaw
    {
		public int DayInt { get; set; }
		public int[] DayRaw { get; set; }
    }

	public class BTWWindow
	{
		public int DayWindowEndInt { get; set; }
		public string DayWindowEndTxt { get; set; }
		public int DayWindowStartInt { get; set; }
		public string DayWindowStartTxt { get; set; }
		public int TimeWindowEndInt { get; set; }
		public string TimeWindowEndTxt { get; set; }
		public int TimeWindowStartInt { get; set; }
		public string TimeWindowStartTxt { get; set; }
		public string WeekWindow { get; set; }
	}

	public class BTVenueInfo
    {
		public string VenueAddress { get; set; }
		public string VenueId { get; set; }
		public string VenueName { get; set; }
		public string VenueTimezone { get; set; }
    }
}
