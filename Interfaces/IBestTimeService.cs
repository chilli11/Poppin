using Poppin.Models.BestTimeEntities;
using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
    interface IBestTimeService
    {
        public BestTimeWeek GetRawWeek(string venueId);
        public BestTimeWeek ForecastRawWeek(string venueId);
        public Task<BestTimeWeek> ForecastRawWeekAsync(PoppinLocation loc);
    }
}
