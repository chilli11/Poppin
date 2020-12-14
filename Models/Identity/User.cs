using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Poppin.Models.Identity
{
	public class User : IdentityUser<Guid>
    {
        public string Role { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens = new List<RefreshToken>();

        public Role RoleEntity { get; set; }
    }
}
