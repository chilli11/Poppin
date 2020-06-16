using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts
{
				public static class ApiRoutes
				{
								public const string Root = "";
								public const string Version = "";
								public const string Base = Root + Version;

								public static class Locations
								{
												public const string Basic = Base + "locations";
												public const string BasicId = Base + "locations/{locationId}";
												public const string GetByYelpSearch = Base + "locations/yelp-search";
								}

								public static class Yelp
								{
												public const string GetBusiness = Base + "yelp/{yelp_id}";
												public const string GetBusinesses = Base + "yelp/businesses";
												public const string GetBusinessMatch = Base + "yelp/match";
												public const string GetBusinessMatchId = Base + "yelp/match/{locId}";
								}

								public static class Identity
								{
												public const string Login = Base + "identity/login";
												public const string Logout = Base + "identity/logout";
												public const string Register = Base + "identity/register";
								}
				}
}
