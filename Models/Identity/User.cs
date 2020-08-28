using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Poppin.Models.Identity
{
				public class User : IdentityUser
    {
        public string Role { get; set; }
        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }

        public Role RoleEntity { get; set; }
    }
}
