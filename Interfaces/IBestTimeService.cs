using Poppin.Models.BestTimeEntities;
using Poppin.Models.BusinessEntities;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
    public interface IBestTimeService
    {
        public BestTimeWeek GetRawWeek(string venueId);
        public BestTimeWeek ForecastRawWeek(string venueId);
        public Task<BestTimeWeek> ForecastRawWeekAsync(PoppinLocation loc);
        public Task StoreForecast(PoppinLocation loc);
    }
}
