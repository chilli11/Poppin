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
								public Task<PoppinLocation> Get(string id);
								public Task<PoppinLocation> CheckExists(PoppinLocation location);
								public Task<List<PoppinLocation>> GetMany(IEnumerable<string> ids);
								public Task<List<PoppinLocation>> GetByYelpList(IEnumerable<string> ids);
								public Task Add(PoppinLocation location);
								public Task Update(string id, PoppinLocation location);
								public Task Update(PoppinLocation location);
								public Task Delete(PoppinLocation location);
								public Task Delete(string id);
				}
}
