using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Poppin.Contracts.Requests;
using Poppin.Interfaces;
using Poppin.Models.Identity;
using Poppin.Models.Tracking;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Poppin.Models.BusinessEntities
{
				[BsonIgnoreExtraElements]
    public class PoppinUser : ProfileEntity
    {
        public PoppinUser(IIdentityService idService) : base(idService) { }


        public PoppinUser(User user) : base(user) { }

        public void Merge(PoppinUserRequest user)
        {
            if (UserId != user.UserId)
            {
                throw new Exception("User ID mismatch");
            }

            Username = user.Username;
            FirstName = user.FirstName;
            LastName = user.LastName;
            Email = user.Email;
            ProfilePhoto = user.ProfilePhoto;
            AgeRange = AgeGroups.Contains(user.AgeRange) ? user.AgeRange : null;
            Gender = Genders.Contains(user.Gender) ? user.Gender : null;
            Categories = new HashSet<string>(user.Categories);
        }

								public string ProfilePhoto { get; set; }
								public string AgeRange { get; set; }
        public string Gender { get; set; }

        public HashSet<string> Categories { get; set; }

								/// <summary>
								/// LocationIds saved by user
								/// </summary>
								public HashSet<string> Favorites { get; set; }

        /// <summary>
        /// LocationIds hidden by user
        /// </summary>
        public HashSet<string> Hidden { get; set; }

        /// <summary>
        /// Vendors that the user is a member of
        /// </summary>
        public HashSet<string> VendorIds { get; set; }

        public async Task<List<Vendor>> GetVendors(IVendorService vs)
        {
            if (VendorIds == null) return null;
            return await vs.GetVendorsByIds(VendorIds);
        }
        public async Task<List<PoppinLocation>> GetFavorites(ILocationService ls)
        {
            if (Favorites == null) return null;
            return await ls.GetMany(Favorites);
        }
        public async Task<List<PoppinLocation>> GetHidden(ILocationService ls)
        {
            if (Hidden == null) return null;
            return await ls.GetMany(Hidden);
        }
        public async Task<IEnumerable<UserLog>> GetuserActivity(ILogActionService las)
        {
            return await las.GetUserActivity(Id);
        }
        private static HashSet<string> AgeGroups
        {
            get
            {
                return new HashSet<string> {
                    { "u18" },
                    { "u26" },
                    { "u36" },
                    { "u46" },
                    { "u56" },
                    { "o55" }
                };
            }
        }
        private static HashSet<string> Genders
        {
            get
            {
                return new HashSet<string> {
                    { "M" },
                    { "F" },
                    { "O" },
                    { "D" }
                };
            }
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum GenderType
    {  
        Unspecified,
        Male,
        Female,
        Other,
        [EnumMember(Value = "Prefer Not to Say")]
        Decline
				}
}
