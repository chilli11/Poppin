using Poppin.Models;
using Poppin.Models.BusinessEntities;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
    public interface IBigDataCloudService
    {
        public Task<PoppinLocation> GetTimeZoneInfo(PoppinLocation loc);
    }
}
