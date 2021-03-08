using MongoDB.Driver;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
using Poppin.Models.Identity;
using Poppin.Models.Tracking;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Services
{
	public class UserService : IUserService
	{
        private readonly IMongoCollection<PoppinUser> _poppinUsers;
		private readonly IMongoCollection<UserLocationRequest> _userLocationRequests;

		public UserService(IMongoDBSettings settings)
		{
			var client = new MongoClient(settings.ConnectionString);
			var database = client.GetDatabase(settings.DatabaseName);

			_poppinUsers = database.GetCollection<PoppinUser>("PoppinUsers");
			_userLocationRequests = database.GetCollection<UserLocationRequest>("UserLocationRequests");
		}
		public Task<PoppinUser> GetUser(string id) =>
			_poppinUsers.FindAsync(p => p.Id == id).Result.FirstOrDefaultAsync();
		public Task<PoppinUser> GetUserById(string userId) =>
			_poppinUsers.FindAsync(p => p.UserId == userId).Result.FirstOrDefaultAsync();
		public Task<PoppinUser> GetUserByEmail(string email) =>
			_poppinUsers.FindAsync(p => p.Email == email).Result.FirstOrDefaultAsync();
		public Task<List<PoppinUser>> GetUsersById(IEnumerable<string> ids) =>
			_poppinUsers.FindAsync(p => ids.Contains(p.UserId)).Result.ToListAsync();

		public Task AddUser(PoppinUser poppinUser) => _poppinUsers.InsertOneAsync(poppinUser);
		public Task UpdateUser(PoppinUser poppinUser) => _poppinUsers.ReplaceOneAsync(v => v.Id == poppinUser.Id, poppinUser);
		public Task UpdateUser(string identityUserId, PoppinUser poppinUser) => _poppinUsers.ReplaceOneAsync(v => v.UserId == identityUserId, poppinUser);
		public Task UpdateUser(User identityUser)
        {
			var profile = GetUserById(identityUser.Id.ToString()).Result;
			profile.User = identityUser;
			return UpdateUser(profile);
        }

		public Task<PoppinUser> CheckExists(PoppinUser poppinUser)
		{
			return _poppinUsers.FindAsync(v => v.Username == poppinUser.Username)
				.Result.FirstOrDefaultAsync();
		}

		public Task AddFavorite(string userId, string locationId)
		{
			var profile = GetUserById(userId).Result;
			if (profile.Favorites == null)
			{
				profile.Favorites = new HashSet<string>();
			}
			profile.Favorites.Add(locationId);
			return UpdateUser(profile);
		}

		public Task RemoveFavorite(string userId, string locationId)
		{
			var profile = GetUserById(userId).Result;
			profile.Favorites.Remove(locationId);
			return UpdateUser(profile);
		}

		public Task HideLocation(string userId, string locationId)
		{
			var profile = GetUserById(userId).Result;
			if (profile.Hidden == null)
			{
				profile.Hidden = new HashSet<string>();
			}
			profile.Hidden.Add(locationId);
			return UpdateUser(profile);
		}

		public Task UnhideLocation(string userId, string locationId)
		{
			var profile = GetUserById(userId).Result;
			profile.Hidden.Remove(locationId);
			return UpdateUser(profile);
		}

		public Task RequestLocation(string userId, PoppinLocation location)
        {
			var profile = GetUserById(userId).Result;
			var req = new UserLocationRequest
			{
				UserId = userId,
				LocationId = location.Id,
				LocationGeo = location.Address.Geo,
				Timestamp = DateTime.Now
			};
			if (profile.RequestedLocations == null) profile.RequestedLocations = new HashSet<string>();
			profile.RequestedLocations.Add(location.Id);

			UpdateUser(profile);
			return _userLocationRequests.InsertOneAsync(req);
        }
	}
}
