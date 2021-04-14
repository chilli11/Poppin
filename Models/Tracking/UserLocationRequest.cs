using MongoDB.Driver.GeoJsonObjectModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Tracking
{
    public class UserLocationRequest
    {
        public string UserId { get; set; }
        public string LocationId { get; set; }
        public DateTime Timestamp { get; set; }
        public GeoJsonPoint<GeoJson2DGeographicCoordinates> LocationGeo { get; set; }
    }
}
