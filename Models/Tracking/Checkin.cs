using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Tracking
{
    public static class ReliabilityScores
    {
        public static double Geo = 0.6;
        public static double Vendor = 1.0;
        public static double User = 1.2;
    }
    public class Checkin
    {
        public Checkin(string locationId, string userId, int visitLength, double reliabilityScore)
        {
            UserId = userId;
            LocationId = locationId;
            Timestamp = DateTime.Now;
            Timeout = DateTime.Now.AddMinutes(visitLength);
            ReliabilityScore = reliabilityScore;
        }

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string LocationId { get; set; }
        public DateTime Timestamp { get; set; }
        public DateTime Timeout { get; set; }
        public double ReliabilityScore { get; set; }
        public bool IsValid
        {
            get
            {
                return DateTime.Now > Timeout;
            }
        }
    }
}
