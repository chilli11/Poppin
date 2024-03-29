﻿using Poppin.Models.BusinessEntities;
using Poppin.Models.Identity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
	public interface IUserService
	{
		public Task<PoppinUser> GetUser(string id);
		public Task<PoppinUser> GetUserById(string id);
		public Task<PoppinUser> GetUserByEmail(string email);
		public Task<List<PoppinUser>> GetUsersById(IEnumerable<string> ids);
		public Task AddUser(PoppinUser poppinUser);
		public Task UpdateUser(PoppinUser poppinUser);
		public Task UpdateUser(string identityUserId, PoppinUser poppinUser);
		public Task UpdateUser(User user);
		public Task<PoppinUser> CheckExists(PoppinUser poppinUser);

		public Task AddFavorite(string userId, string locationId);
		public Task RemoveFavorite(string userId, string locationId);
		public Task HideLocation(string userId, string locationId);
		public Task UnhideLocation(string userId, string locationId);
		public Task RequestLocation(string userId, PoppinLocation location);
	}
}
