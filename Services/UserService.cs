using MongoDB.Driver;
using Poppin.Configuration;
using Poppin.Interfaces;
using Poppin.Models.BusinessEntities;
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

								public UserService(IMongoDBSettings settings)
								{
												var client = new MongoClient(settings.ConnectionString);
												var database = client.GetDatabase(settings.DatabaseName);

												_poppinUsers = database.GetCollection<PoppinUser>("PoppinUsers");
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

								public Task<PoppinUser> CheckExists(PoppinUser poppinUser)
								{
												return _poppinUsers.FindAsync(v => v.Username == poppinUser.Username)
																		.Result.FirstOrDefaultAsync();
								}

								public Task AddFavorite(string userId, string locationId)
								{
												var profile = GetUserById(userId).Result;
												if (!profile.Favorites.Contains(locationId))
												{
																profile.Favorites.Append(locationId);
																return UpdateUser(profile);
												}
												return Task.FromResult(0);
								}

								public Task RemoveFavorite(string userId, string locationId)
								{
												var profile = GetUserById(userId).Result;
												if (profile.Favorites.Contains(locationId))
												{
																profile.Favorites = profile.Favorites.Where(l => l != locationId).ToHashSet();
																return UpdateUser(profile);
												}
												return Task.FromResult(0);
								}

								public Task HideLocation(string userId, string locationId)
								{
												var profile = GetUserById(userId).Result;
												if (!profile.Hidden.Contains(locationId))
												{
																profile.Hidden.Append(locationId);
																return UpdateUser(profile);
												}
												return Task.FromResult(0);
								}

								public Task UnhideLocation(string userId, string locationId)
								{
												var profile = GetUserById(userId).Result;
												if (profile.Hidden.Contains(locationId))
												{
																profile.Hidden = profile.Hidden.Where(l => l != locationId).ToHashSet();
																return UpdateUser(profile);
												}
												return Task.FromResult(0);
								}
				}
}
