using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Tracking
{
				public enum ActionTypes
				{
								Search,
								ViewLocation,
								SaveFavorite,
								RemoveFavorite,
								HideLocation,
								UnhideLocation,
								UpdateProfile,
								Checkin,
								UpdateGeo,
								GetDirections
				}

				public class UserLog
				{
								public UserLog()
								{
												Date = DateTime.Today.ToString();
								}
								public string UserId { get; set; }
								public string Date { get; set; }
								public IEnumerable<LogEntry> Entries { get; set; }
				}

				public class LogEntry
				{
								public LogEntry()
								{
												Timestamp = DateTime.Now;
								}

								public DateTime Timestamp { get; set; }
								public int ActionType { get; set; }
								public LogAction Action { get; set; }
				}
}
