﻿using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.GeoJsonObjectModel;
using Pluralize.NET;
using Poppin.Configuration;
using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using Poppin.Models;
using Poppin.Models.BusinessEntities;
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
		private readonly IMongoCollection<Category> _categories;
		IPluralize pluralizer = new Pluralizer();

		public LocationService(IMongoDBSettings settings)
		{
			var client = new MongoClient(settings.ConnectionString);
			var database = client.GetDatabase(settings.DatabaseName);

			_locations = database.GetCollection<PoppinLocation>("Locations");
			_checkins = database.GetCollection<Checkin>("Checkins");
			_categories = database.GetCollection<Category>("Categories");
		}

		public Task<PoppinLocation> Get(string id) => _locations.Find(loc => loc.Id == id).FirstAsync();
		public Task<List<PoppinLocation>> GetMany(IEnumerable<string> ids) =>
			_locations.Find(loc => ids.Contains(loc.Id)).ToListAsync();
		public Task<PoppinLocation> CheckExists(PoppinLocation location) =>
			_locations.Find(l => l.Address.Line1 == location.Address.Line1 && l.Name == location.Name)
				.FirstOrDefaultAsync();

		public Task<List<PoppinLocation>> GetByYelpList(IEnumerable<string> ids) =>
			_locations.Find(loc => ids.Contains(loc.YelpId)).ToListAsync();

		public async Task<List<PoppinLocation>> GetBySearch(LocationSearchRequest request)
		{
			List<PoppinLocation> locations;
			string[] split = request.Term.Split(' ');
			var Terms = split.Concat(split.Select(w => Pluralize(w)));
			var text = Builders<PoppinLocation>.Filter.Text(String.Join(" ", Terms));

			var x = new GeoJson2DGeographicCoordinates(request.Geo.Coordinates[0], request.Geo.Coordinates[1]);
			var geo = new GeoJsonPoint<GeoJson2DGeographicCoordinates>(x);
			var inRange = Builders<PoppinLocation>.Filter.Near(a => a.Address.Geo, geo, request.Radius);

			var Categories = await ExpandCategories(request.Categories, request.CategorySlugs);
			var cats = request.Categories.SelectMany(c => c.Children);
			cats = cats.Concat(request.Categories.Select(c => c.Slug));
			cats = cats.Concat(Terms);

			if (cats != null && cats.Count() > 0)
			{
				var matchCat = Builders<PoppinLocation>.Filter.AnyIn("Categories", cats);
				locations = await _locations.Find(inRange & matchCat).ToListAsync();
			}
			locations = await _locations.Find(inRange).ToListAsync();
			return locations;
		}

		public Task Add(PoppinLocation location)
		{
			location.YelpDetails = null;
			location.LastUpdate = DateTime.UtcNow;
			return _locations.InsertOneAsync(location);
		}

		public Task Update(string id, PoppinLocation location)
		{
			location.YelpDetails = null;
			location.LastUpdate = DateTime.UtcNow;
			return _locations.ReplaceOneAsync(l => l.Id == location.Id, location);
		}
		public Task Update(PoppinLocation location)
		{
			location.YelpDetails = null;
			location.LastUpdate = DateTime.UtcNow;
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
		public bool ReconcileCheckin(Checkin c)
		{
			List<Checkin> checkins;
			FilterDefinition<Checkin> filter;
			if (c.ReliabilityScore == ReliabilityScores.User)
			{
				filter = Builders<Checkin>.Filter.Gt("Timestamp", DateTime.Now.AddSeconds(-90))
					& Builders<Checkin>.Filter.Eq("LocationId", c.LocationId)
					& Builders<Checkin>.Filter.Eq<string>("UserId", null);
				checkins = _checkins.Find(filter).ToList();
				if (checkins.Count > 0)
				{
					var replace = checkins.FirstOrDefault();
					replace.Timeout = DateTime.Now;
					_checkins.ReplaceOneAsync(c => c.Id == replace.Id, replace);
				}
			}
			else
			{
				filter = Builders<Checkin>.Filter.Gt("Timestamp", DateTime.Now.AddSeconds(-90))
					& Builders<Checkin>.Filter.Eq("LocationId", c.LocationId)
					& Builders<Checkin>.Filter.Ne<string>("UserId", null);
				checkins = _checkins.Find(filter).ToList();
				if (checkins.Count > 0)
				{
					return false;
				}
			}
			return true;
		}

		public UpdateResult InvalidateCheckin(string userId)
		{
			var filter = Builders<Checkin>.Filter.Gt("Timeout", DateTime.Now) & Builders<Checkin>.Filter.Eq("UserId", userId);
			var update = Builders<Checkin>.Update.Set("Timeout", DateTime.Now);
			return _checkins.UpdateMany(filter, update);
		}
		public Task InvalidateVendorCheckin(string locId)
		{
			var filter = Builders<Checkin>.Filter.Gt("Timeout", DateTime.Now)
				& Builders<Checkin>.Filter.Eq("LocationId", locId)
				& Builders<Checkin>.Filter.Eq<string>("UserId", null);
			var checkins = _checkins.Find(filter).ToList();
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
			_checkins.Find(Builders<Checkin>.Filter.Gt("Timeout", DateTime.Now) & Builders<Checkin>.Filter.Eq("LocationId", locId)).ToListAsync();
		public Task<List<Checkin>> GetCheckinsForLocations(IEnumerable<string> locIds) =>
			_checkins.Find(Builders<Checkin>.Filter.Gt("Timeout", DateTime.Now) & Builders<Checkin>.Filter.Where(c => locIds.Contains(c.LocationId))).ToListAsync();
		public Task<List<Checkin>> GetCheckinsForUser(string uId) => _checkins.Find(Builders<Checkin>.Filter.Eq("UserId", uId)).ToListAsync();


		// ================ CATEGORIES ================== //
		public Task<List<Category>> GetCategories() => _categories.Find(new BsonDocument()).ToListAsync();
		public Task<Category> GetCategoryBySlug(string slug) => _categories.Find(c => c.Slug == slug).FirstAsync();
		public Task<List<Category>> GetCategoriesBySlug(IEnumerable<string> slugs) => _categories.Find(c => slugs.Contains(c.Slug)).ToListAsync();
		public Task<Category> GetCategoryByHereId(string hereId) => _categories.Find(c => c.HereId == hereId).FirstAsync();

		public Task AddCategory(Category category) => _categories.InsertOneAsync(category);
		public Task AddCategories(IEnumerable<Category> categories) => _categories.InsertManyAsync(categories);
		public Task UpdateCategory(Category category) => _categories.ReplaceOneAsync(c => c.Slug == category.Slug, category);
		public Task UpdateCategory(string catSlug, Category category) => _categories.ReplaceOneAsync(c => c.Slug == catSlug, category);
		public void UpdateCategories(IEnumerable<Category> categories) => categories.ToList().ForEach(c => UpdateCategory(c));
		public Task DeleteCategory(string catSlug) => _categories.DeleteOneAsync(c => c.Slug == catSlug);

		private string Pluralize(string word)
		{
			return pluralizer.IsSingular(word) ? pluralizer.Pluralize(word) : pluralizer.Singularize(word);
		}

		private async Task<List<Category>> ExpandCategories(IEnumerable<Category> categories, IEnumerable<string> slugs)
        {
			List<Category> cats;
			if ((categories == null || categories.Count() == 0) && slugs != null && slugs.Count() > 0)
			{
				cats = await GetCategoriesBySlug(slugs);
				categories = cats.ToHashSet();
			}
			else if (categories != null && categories.Count() > 0 && categories.First().Id == null)
			{
				slugs = categories.Select(c => c.Slug).ToHashSet();
				cats = await GetCategoriesBySlug(slugs);
				categories = cats.ToHashSet();
			}
			else
            {
				cats = new List<Category>();
            }

			return cats;
		}
	}

	public static class LocationExtensions
	{
		public static List<PoppinLocation> FilterByCrowdOver(this List<PoppinLocation> _list, double threshhold) =>
			_list.FindAll(loc => loc.CrowdSize / loc.Capacity > threshhold);
		public static List<PoppinLocation> FilterByCrowdUnder(this List<PoppinLocation> _list, double threshhold) =>
			_list.FindAll(loc => loc.CrowdSize / loc.Capacity < threshhold);
	}
}
