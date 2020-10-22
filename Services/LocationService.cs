using MongoDB.Driver;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models;
using Poppin.Models.Tracking;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Services
{
				public class LocationService : ILocationService
				{
								private readonly IMongoCollection<PoppinLocation> _locations;
								private readonly IMongoCollection<Checkin> _checkins;

								public LocationService(IMongoDBSettings settings)
								{
												var client = new MongoClient(settings.ConnectionString);
												var database = client.GetDatabase(settings.DatabaseName);

												_locations = database.GetCollection<PoppinLocation>("Locations");
												_checkins = database.GetCollection<Checkin>("Checkins");
								}

								public Task<PoppinLocation> Get(string id) => _locations.Find(loc => loc.Id == id).FirstAsync();
								public Task<List<PoppinLocation>> GetMany(IEnumerable<string> ids) =>
												_locations.Find(loc => ids.Contains(loc.Id)).ToListAsync();
								public Task<PoppinLocation> CheckExists(PoppinLocation location) =>
												_locations.Find(l => l.Address.Line1 == location.Address.Line1 && l.Name == location.Name)
																.FirstOrDefaultAsync();

								public Task<List<PoppinLocation>> GetByYelpList(IEnumerable<string> ids) =>
												_locations.Find(loc => ids.Contains(loc.YelpId)).ToListAsync();

								public Task Add(PoppinLocation location)
								{
												location.YelpDetails = null;
												return _locations.InsertOneAsync(location);
								}

								public Task Update(string id, PoppinLocation location)
								{
												location.YelpDetails = null;
												return _locations.ReplaceOneAsync(l => l.Id == location.Id, location);
								}
								public Task Update(PoppinLocation location)
								{
												location.YelpDetails = null;
												return _locations.ReplaceOneAsync(l => l.Id == location.Id, location);
								}
								public Task Delete(PoppinLocation location) => _locations.DeleteOneAsync(loc => loc.Id == location.Id);
								public Task Delete(string id) => _locations.DeleteOneAsync(loc => loc.Id == id);

								// ========== CHECKINS =========== //

								public Task NewCheckin(Checkin c)
								{
												if (c.UserId != null) InvalidateCheckin(c.UserId);
												return _checkins.InsertOneAsync(c);
								}

								public UpdateResult InvalidateCheckin(string userId)
								{
												var filter = Builders<Checkin>.Filter.Eq("UserId", userId) & Builders<Checkin>.Filter.Gt("Timeout", DateTime.Now);
												var update = Builders<Checkin>.Update.Set("Timeout", DateTime.Now);
												return _checkins.UpdateMany(filter, update);
								}
								public Task InvalidateVendorCheckin(string locId)
								{
												var checkins = _checkins.Find(c => c.LocationId == locId && c.UserId == null && c.Timeout > DateTime.Now).ToList();
												if (checkins.Count > 0)
												{
																checkins.Sort((a, b) => a.Timeout.CompareTo(b.Timeout));
																var replace = checkins.FirstOrDefault();
																replace.Timeout = DateTime.Now;
																return _checkins.ReplaceOneAsync(c => c.Id == replace.Id, replace);
												}
												return null;
								}
								public Task<List<Checkin>> GetCheckinsForLocation(string locId) =>
												_checkins.Find(c => c.LocationId == locId && c.Timeout > DateTime.Now).ToListAsync();
								public Task<List<Checkin>> GetCheckinsForUser(string uId) => _checkins.Find(c => c.UserId == uId).ToListAsync();
				}

				public static class LocationExtensions
				{
								public static List<PoppinLocation> FilterByCrowdOver(this List<PoppinLocation> _list, double threshhold) =>
												_list.FindAll(loc => loc.CrowdSize / loc.Capacity > threshhold);
								public static List<PoppinLocation> FilterByCrowdUnder(this List<PoppinLocation> _list, double threshhold) =>
												_list.FindAll(loc => loc.CrowdSize / loc.Capacity < threshhold);
				}
}
