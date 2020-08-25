using Poppin.Models.BusinessEntities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface IUserService
				{
								public Task<PoppinUser> GetUserById(string id);
								public Task<List<PoppinUser>> GetUsersById(IEnumerable<string> ids);
								public Task AddUser(PoppinUser poppinUser);
								public Task UpdateUser(PoppinUser poppinUser);
								public Task UpdateUser(string poppinUserId, PoppinUser poppinUser);
								public Task<PoppinUser> CheckExists(PoppinUser poppinUser);

								public Task AddFavorite(string userId, string locationId);
								public Task RemoveFavorite(string userId, string locationId);
								public Task HideLocation(string userId, string locationId);
								public Task UnhideLocation(string userId, string locationId);
				}
}
