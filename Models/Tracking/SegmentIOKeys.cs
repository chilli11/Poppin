namespace Poppin.Models.Tracking
{
    public static class SegmentIOKeys
    {
        public const string AnonId = "000";
        public static class Categories
        {
            public const string Identity = "Identity";
            public const string Profile = "Profile";
            public const string Location = "Location";
            public const string Vendor = "Vendor";
        }
        public static class Actions
        {
            public const string Register = "Register";
            public const string Login = "Login";
            public const string UpdateProfile = "UpdateProfile";
            public const string ViewUserProfile = "ViewUserProfile";
            public const string ConfirmEmail = "ConfirmEmail";
            public const string ResendConfirmationEmail = "ResendConfirmationEmail";
            public const string StartPasswordReset = "StartPassswordReset";
            public const string CompletePasswordReset = "CompletePasswordReset";

            public const string Search = "Search";
            public const string ViewLocation = "ViewLocation";
            public const string ViewVendor = "ViewVendor";
            public const string ViewVendorList = "ViewVendorList";

            public const string AddFavorite = "AddFavorite";
            public const string RemoveFavorite = "RemoveFavorite";
            public const string RequestLocation = "RequestLocation";
            public const string HideLocation = "HideLocation";
            public const string UnhideLocation = "UnhideLocation";
            public const string Checkin = "Checkin";
            public const string UpdateGeo = "UpdateGeo";
            public const string GetDirections = "GetDirections";

            public const string AddVendor = "AddVendor";
            public const string AddLocation = "AddLocation";

            public const string UpdateLocation = "UpdateLocation";
            public const string DeleteLocation = "DeleteLocation";
            public const string IncrementCrowd = "IncrementCrowd";
            public const string DecrementCrowd = "DecrementCrowd";

            public const string AddVendorMember = "AddVendorMember";
            public const string AddVendorAdmin = "AddVendorAdmin";
            public const string AddVendorLocation = "AddVendorLocation";
            public const string AddSubVendor = "AddSubVendor";
            public const string RemoveVendorMember = "RemoveVendorMember";
            public const string RemoveVendorAdmin = "RemoveVendorAdmin";
            public const string RemoveVendorLocation = "RemoveVendorLocation";
            public const string RemoveSubVendor = "RemoveSubVendor";
        }
    }
}
