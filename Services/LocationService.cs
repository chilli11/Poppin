using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poppin.Models;
using Poppin.Interfaces;
using Poppin.Models.Yelp;
using Poppin.Configuration;

namespace Poppin.Services
{
				public class LocationService : ILocationService
				{
								private readonly IMongoCollection<PoppinLocation> _locations;

								public LocationService(IMongoDBSettings settings)
								{
												var client = new MongoClient(settings.ConnectionString);
												var database = client.GetDatabase(settings.DatabaseName);

												_locations = database.GetCollection<PoppinLocation>("Locations");
								}

								public Task<PoppinLocation> Get(string id) => _locations.FindAsync(loc => loc.Id == id).Result.FirstAsync();
								public Task<List<PoppinLocation>> GetMany(IEnumerable<string> ids) => _locations.FindAsync(loc => ids.Contains(loc.Id))
												.Result.ToListAsync();
								public Task<PoppinLocation> CheckExists(PoppinLocation location)
								{
												return _locations.FindAsync(l => l.Address.Line1 == location.Address.Line1 && l.Name == location.Name)
																.Result.FirstOrDefaultAsync();
								}

								public Task<List<PoppinLocation>> GetByYelpList(IEnumerable<string> ids) =>
												_locations.FindAsync(loc => ids.Contains(loc.YelpId)).Result.ToListAsync();

								public Task Add(PoppinLocation location)
								{
												location.YelpDetails = null;
												return _locations.InsertOneAsync(location);
								}

								public Task Update(string id, PoppinLocation location)
								{
												location.YelpDetails = null;
												return _locations.ReplaceOneAsync(loc => loc.Id == id, location);
								}
								public Task Update(PoppinLocation location)
								{
												location.YelpDetails = null;
												return _locations.ReplaceOneAsync(loc => loc.Id == location.Id, location);
								}
								public Task Delete(PoppinLocation location) => _locations.DeleteOneAsync(loc => loc.Id == location.Id);
								public Task Delete(string id) => _locations.DeleteOneAsync(loc => loc.Id == id);
				}

				public static class LocationExtensions
				{
								public static List<PoppinLocation> FilterByCrowdOver(this List<PoppinLocation> _list, double threshhold) =>
												_list.FindAll(loc => loc.CrowdSize / loc.Capacity > threshhold);
								public static List<PoppinLocation> FilterByCrowdUnder(this List<PoppinLocation> _list, double threshhold) =>
												_list.FindAll(loc => loc.CrowdSize / loc.Capacity < threshhold);
				}
}
