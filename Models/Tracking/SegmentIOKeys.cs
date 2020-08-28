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

												public const string Search = "Search";
												public const string ViewLocation = "ViewLocation";


												public const string AddFavorite = "AddFavorite";
												public const string RemoveFavorite = "RemoveFavorite";
												public const string HideLocation = "HideLocation";
												public const string UnhideLocation = "UnhideLocation";
												public const string Checkin = "Checkin";
												public const string UpdateGeo = "UpdateGeo";
												public const string GetDirections = "GetDirections";

												public const string AddVendor = "AddVendor";
												public const string AddMember = "AddMember";
												public const string AddLocation = "AddLocation";
												public const string UpdateLocation = "UpdateLocation";
												public const string DeleteLocation = "DeleteLocation";
												public const string IncrementCrowd = "IncrementCrowd";
												public const string DecrementCrowd = "DecrementCrowd";
								}
				}
}
