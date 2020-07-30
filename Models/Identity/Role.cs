using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity
{
				public class Role : IdentityRole
				{
								public const string Admin = "Admin";
								public const string Vendor = "Vendor";
								public const string User = "User";
				}
}
