using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity
{
				[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, Inherited = true, AllowMultiple = true)]
				public class AuthorizeRolesAttribute : AuthorizeAttribute
				{
								public AuthorizeRolesAttribute(params string[] roles) : base()
								{
												AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme;
												Roles = roles.Count() > 0 ? string.Join(",", roles) : null;
								}
				}
}
