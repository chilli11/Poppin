using Newtonsoft.Json;
using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.BestTimeEntities
{
	public class BestTimeWeek : IForecast
	{
		[JsonProperty("analysis")]
		public BTWAnalysis Analysis { get; set; }
		[JsonProperty("epoch_analysis")]
		public int EpochAnalysis { get; set; }
		[JsonProperty("venue_name")]
		public string VenueName { get; set; }
		[JsonProperty("window")]
		public BTWWindow Window { get; set; }
		[JsonProperty("forecast_updated_on")]
		public DateTime ForecastUpdatedOn { get; set; }
		[JsonProperty("venue_info")]
		public BTVenueInfo VenueInfo { get; set; }

		public int GetForecastOccupancy(PoppinTimeZone tz)
		{
			if (Analysis == null || Analysis.WeekRaw == null) return 0;

			var now = DateTime.Now.AddSeconds(tz.UtcOffsetSeconds);
			var day = FixDay(now.Hour, (int)now.DayOfWeek);
			var hour = FixHour(now.Hour);
			var dayRaw = (BTWDayRaw)Analysis.WeekRaw.GetValue(day);
			return (int)dayRaw.DayRaw.GetValue(hour);

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
		[JsonProperty("week_raw")]
		public BTWDayRaw[] WeekRaw { get; set; }
	}

	public class BTWDayRaw
	{
		[JsonProperty("day_int")]
		public int DayInt { get; set; }
		[JsonProperty("day_raw")]
		public int[] DayRaw { get; set; }
    }

	public class BTWWindow
	{
		[JsonProperty("day_window_end_int")]
		public int DayWindowEndInt { get; set; }
		[JsonProperty("day_window_end_txt")]
		public string DayWindowEndTxt { get; set; }
		[JsonProperty("day_window_start_int")]
		public int DayWindowStartInt { get; set; }
		[JsonProperty("day_window_start_txt")]
		public string DayWindowStartTxt { get; set; }
		[JsonProperty("time_window_end")]
		public int TimeWindowEndInt { get; set; }
		[JsonProperty("time_window_end_12h")]
		public string TimeWindowEndTxt { get; set; }
		[JsonProperty("time_window_start")]
		public int TimeWindowStartInt { get; set; }
		[JsonProperty("time_window_start_12h")]
		public string TimeWindowStartTxt { get; set; }
		[JsonProperty("week_window")]
		public string WeekWindow { get; set; }
	}

	public class BTVenueInfo
	{
		[JsonProperty("venue_address")]
		public string VenueAddress { get; set; }
		[JsonProperty("venue_id")]
		public string VenueId { get; set; }
		[JsonProperty("venue_name")]
		public string VenueName { get; set; }
		[JsonProperty("venue_timezone")]
		public string VenueTimezone { get; set; }
    }
}
