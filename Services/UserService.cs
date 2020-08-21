using MongoDB.Driver;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Services
{
				public class UserService : IUserService
				{
								private readonly IMongoCollection<PoppinUser> _poppinUsers;

								public UserService(IMongoDBSettings settings)
								{
												var client = new MongoClient(settings.ConnectionString);
												var database = client.GetDatabase(settings.DatabaseName);

												_poppinUsers = database.GetCollection<PoppinUser>("PoppinUsers");
								}
								public Task<PoppinUser> GetUserById(string id) =>
												_poppinUsers.FindAsync(p => p.UserId == id).Result.FirstOrDefaultAsync();
								public Task<List<PoppinUser>> GetUsersById(IEnumerable<string> ids) =>
												_poppinUsers.FindAsync(p => ids.Contains(p.UserId)).Result.ToListAsync();

								public Task AddUser(PoppinUser poppinUser) => _poppinUsers.InsertOneAsync(poppinUser);
								public Task UpdateUser(PoppinUser poppinUser) => _poppinUsers.ReplaceOneAsync(v => v.Id == poppinUser.Id, poppinUser);
								public Task UpdateUser(string poppinUserId, PoppinUser poppinUser) => _poppinUsers.ReplaceOneAsync(v => v.Id == poppinUserId, poppinUser);

								public Task<PoppinUser> CheckExists(PoppinUser poppinUser)
								{
												return _poppinUsers.FindAsync(v => v.Username == poppinUser.Username)
																		.Result.FirstOrDefaultAsync();
								}
				}
}
