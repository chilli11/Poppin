using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Poppin.Models.Identity;

namespace Poppin.Models.BusinessEntities
{
				public class ProfileEntity
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        //[ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [PersonalData]
        public string Email { get; set; }
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
    }
}
