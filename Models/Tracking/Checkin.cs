using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Tracking
{
				public class Checkin
				{
								public Checkin(string locationId, string userId, int visitLength)
								{
												UserId = UserId;
												LocationId = locationId;
												Timestamp = DateTime.Now;
												Timeout = DateTime.Now.AddMinutes(visitLength);
								}

								[BsonId]
								[BsonRepresentation(BsonType.ObjectId)]
								public string Id { get; set; }
								public string UserId { get; set; }
								public string LocationId { get; set; }
								public DateTime Timestamp { get; set; }
								public DateTime Timeout { get; set; }
								public float ReliabilityScore { get; set; }
								public bool IsValid
								{
												get
												{
																return Timeout > Timestamp;
												}
								}
				}
}
