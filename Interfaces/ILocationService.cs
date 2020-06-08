using Poppin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poppin.Models.Yelp;

namespace Poppin.Interfaces
{
				public interface ILocationService
				{
								public PoppinLocation Get(string id);
								public PoppinLocation CheckExists(PoppinLocation location);
								public List<PoppinLocation> GetByYelpList(List<YelpBusiness> businesses);
								public List<PoppinLocation> GetByCity(string city);
								public List<PoppinLocation> GetByZip(int zipCode);
								public Task Add(PoppinLocation location);
								public Task Update(string id, PoppinLocation location);
								public Task Update(PoppinLocation location);
								public void Delete(PoppinLocation location);
								public void Delete(string id);
				}
}
