using MongoDB.Driver;
using Poppin.Contracts.Requests;
using Poppin.Models;
using Poppin.Models.Tracking;
using System.Collections.Generic;
using System.Security.Cryptography.Pkcs;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface ILocationService
				{
								public Task<PoppinLocation> Get(string id);
								public Task<PoppinLocation> CheckExists(PoppinLocation location);
								public Task<List<PoppinLocation>> GetMany(IEnumerable<string> ids);
								public Task<List<PoppinLocation>> GetByYelpList(IEnumerable<string> ids);
								public Task<List<PoppinLocation>> GetBySearch(LocationSearchRequest request);
								public Task Add(PoppinLocation location);
								public Task Update(string id, PoppinLocation location);
								public Task Update(PoppinLocation location);
								public Task Delete(PoppinLocation location);
								public Task Delete(string id);
								public Task NewCheckin(Checkin c);
								public UpdateResult InvalidateCheckin(string userId);
								public Task InvalidateVendorCheckin(string locId);
								public Task<List<Checkin>> GetCheckinsForLocation(string locId);
								public Task<List<Checkin>> GetCheckinsForUser(string uId);
								public Task<List<Category>> GetCategories();
								public Task<Category> GetCategoryBySlug(string slug);
								public Task<List<Category>> GetCategoriesBySlug(IEnumerable<string> slugs);
								public Task<Category> GetCategoryByHereId(string hereId);
								public Task AddCategory(Category category);
								public Task AddCategories(IEnumerable<Category> categories);
								public Task UpdateCategory(Category category);
								public Task UpdateCategory(string catSlug, Category category);
								public void UpdateCategories(IEnumerable<Category> categories);
								public Task DeleteCategory(string catSlug);
				}
}
