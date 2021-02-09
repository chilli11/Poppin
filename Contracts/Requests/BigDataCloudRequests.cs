using Poppin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
    public class BigDataCloudTZRequest
    {
        public BigDataCloudTZRequest(Coord coords, string key)
        {
            Latitude = coords.Latitude.ToString();
            Longitude = coords.Longitude.ToString();
            Key = key;
        }

        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public int UtcReference = 0;
        public string Key { get; set; }
    }
}
