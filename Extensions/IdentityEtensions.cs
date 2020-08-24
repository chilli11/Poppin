using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Extensions
{
				public static class IdentityEtensions
				{
								public static string GetUserId(this HttpContext httpContext)
								{
												if (httpContext.User == null)
												{
																return string.Empty;
												}

												return httpContext.User.Claims.Single(u => u.Type == "Id").Value;
								}

								public static string GetUserRole(this HttpContext httpContext)
								{
												if (httpContext.User == null)
												{
																return string.Empty;
												}

												return httpContext.User.Claims.Single(u => u.Type == "Role").Value;
								}
				}
}
