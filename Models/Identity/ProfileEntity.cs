using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity
{
				public class ProfileEntity
    {
        public string UserId { get; set; }
        public string Username { get; set; }
        public string Role { get; set; }

        [ForeignKey(nameof(UserId))]
        public User User { get; set; }

        [PersonalData]
        public string FirstName { get; set; }
        [PersonalData]
        public string LastName { get; set; }
    }
}
