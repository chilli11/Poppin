﻿using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Poppin.Models;
using Poppin.Interfaces;

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

								public PoppinLocation Get(string id) => _locations.Find(loc => loc.Id == id).First();
								public PoppinLocation CheckExists(PoppinLocation location)
								{
												return _locations.Find(l => l.Address.Line1 == location.Address.Line1 && l.Name == location.Name).FirstOrDefault();
								}
								public List<PoppinLocation> GetByCity(string city) =>
												_locations.Find(loc => loc.Address.City.ToLower() == city.ToLower()).ToList();
								public List<PoppinLocation> GetByZip(int zipCode) =>
												_locations.Find(loc => loc.Address.ZipCode == zipCode).ToList();

								public Task Add(PoppinLocation location) => _locations.InsertOneAsync(location);
								public void Update(string id, PoppinLocation location) => _locations.ReplaceOne(loc => loc.Id == id, location);
								public void Update(PoppinLocation location) => _locations.ReplaceOne(loc => loc.Id == location.Id, location);
								public void Delete(PoppinLocation location) => _locations.DeleteOne(loc => loc.Id == location.Id);
								public void Delete(string id) => _locations.DeleteOne(loc => loc.Id == id);
				}

				public static class LocationExtensions
				{
								public static List<PoppinLocation> FilterByCategory(this List<PoppinLocation> _list, string category) =>
												_list.FindAll(loc => loc.Categories.Contains(category));
								public static List<PoppinLocation> FilterByCrowdOver(this List<PoppinLocation> _list, double threshhold) =>
												_list.FindAll(loc => loc.CrowdSize / loc.Capacity > threshhold);
								public static List<PoppinLocation> FilterByCrowdUnder(this List<PoppinLocation> _list, double threshhold) =>
												_list.FindAll(loc => loc.CrowdSize / loc.Capacity < threshhold);
				}
}
