using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Tracking
{
				public static class SegmentIOKeys
				{
								public static class Categories
								{
												public const string Identity = "Identity";
								}
								public static class Actions
								{
												public const string Register = "Register";
												public const string Login = "Login";
												public const string UpdateProfile = "UpdateProfile";
												public const string ViewUserProfile = "ViewUserProfile";

												public const string AddFavorite = "AddFavorite";
												public const string RemoveFavorite = "RemoveFavorite";
												public const string HideLocation = "HideLocation";
												public const string UnhideLocation = "UnhideLocation";
								}
				}
}
