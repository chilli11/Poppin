using Poppin.Models;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Geocoding.HERE;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
	public interface IHEREGeocoder
	{
		public Task<Place> Lookup(IDictionary<string, string> queryParams);
		public Task<Geocode> Geocode(IDictionary<string, string> queryParams);
		public Task<IEnumerable<Place>> Discover(IDictionary<string, string> queryParams, GeoCoords at);
		public Task<IEnumerable<Place>> Browse(IDictionary<string, string> queryParams, GeoCoords at);
	}
}
