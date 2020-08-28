using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
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
								GetDirections,

								AddVendor,
								AddMember,
								AddLocation,
								UpdateLocation,
								DeleteLocation,
								IncrementCrowd,
								DecrementCrowd
				}

				public class UserLog
				{
								public UserLog()
								{
												Date = DateTime.Today;
								}

								[BsonId]
								[BsonRepresentation(BsonType.ObjectId)]
								public string Id { get; set; }
								public string UserId { get; set; }
								public DateTime Date { get; set; }
								public List<LogEntry> Entries { get; set; }
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
