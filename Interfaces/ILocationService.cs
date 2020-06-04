using Poppin.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface ILocationService
				{
								public PoppinLocation Get(string id);
								public PoppinLocation CheckExists(PoppinLocation location);
								public List<PoppinLocation> GetByCity(string city);
								public List<PoppinLocation> GetByZip(int zipCode);
								public Task Add(PoppinLocation location);
								public void Update(string id, PoppinLocation location);
								public void Update(PoppinLocation location);
								public void Delete(PoppinLocation location);
								public void Delete(string id);
				}
}
