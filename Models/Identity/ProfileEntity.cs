using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity
{
				public class ProfileEntity
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Username { get; set; }

        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
    }
}
