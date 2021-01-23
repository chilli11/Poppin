using Poppin.Models;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Geocoding.HERE;
using System.Collections.Generic;

namespace Poppin.Interfaces
{
	public interface IHEREGeocoder
	{
		public Place Lookup(IDictionary<string, string> queryParams);
		public Geocode Geocode(IDictionary<string, string> queryParams);
		public IEnumerable<Place> Discover(IDictionary<string, string> queryParams, GeoCoords at);
		public IEnumerable<Place> Browse(IDictionary<string, string> queryParams, GeoCoords at);
	}
}
