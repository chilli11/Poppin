using Poppin.Models.Yelp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface IYelpService
				{
								public Task<List<YelpCategory>> GetCategory();
								public Task<YelpCategory> GetCategory(string alias);
								public Task<YelpBusiness> GetBusiness(string id);
								public Task<YelpBusinessSearchResponse> GetBusinessSearch(IYelpSearchParams searchParams);
								public Task<YelpBusinessSearchResponse> GetBusinessMatch(IYelpSearchParams searchParams);
				}
}
