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
								SaveLocation,
								HideLocation,
								UpdateProfile,
								Checkin,
								GetDirections
				}

				public class UserLog
				{
								public string UserId { get; set; }
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
