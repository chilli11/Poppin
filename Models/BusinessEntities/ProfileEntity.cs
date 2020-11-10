using Microsoft.AspNetCore.Identity;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Poppin.Interfaces;
using Poppin.Models.Identity;

namespace Poppin.Models.BusinessEntities
{
				public class ProfileEntity
    {
        public ProfileEntity(IIdentityService idService)
								{
            _identityService = idService;
								}

        public ProfileEntity(User user)
        {
            User = user;
            UserId = user.Id.ToString();
            Username = user.UserName;
            Role = user.Role;
            Email = user.Email;
        }

        private IIdentityService _identityService;

        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        private User _user;
        public User User
        {
            get
            {
                if (_user != null) return _user;
                _user = _identityService.GetUserById(UserId).Result.User;
                return _user;
            }
            set
            {
                _user = value;
            }
        }

        [PersonalData]
        public string Email { get; set; }
        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
    }
}
