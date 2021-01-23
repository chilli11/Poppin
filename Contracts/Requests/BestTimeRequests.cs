using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
    public class BestTimeRequest
    {
        public BestTimeRequest(PoppinLocation loc, string privateKey)
        {
            api_key_private = privateKey;
            venue_name = loc.Name;
            venue_address = $"{loc.Address.Line1}, {loc.Address.City}, {loc.Address.State}";
        }

        public string api_key_private { get; set; }
        public string venue_name { get; set; }
        public string venue_address { get; set; }
    }
    public class BestTimeIDRequest
    {
        public string api_key_public { get; set; }
        public string venue_id { get; set; }
    }
}
