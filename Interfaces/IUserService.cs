using Poppin.Models.Identity;
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
				}
}
