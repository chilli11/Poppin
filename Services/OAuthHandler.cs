using Poppin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Services
{
				public class OAuthHandler : IOAuthHandler
				{
								public OAuthHandler(FBAuthService fBAuthService, GoogleAuthService googleAuthService)
								{
												Services.Add("Facebook", fBAuthService);
												Services.Add("Google", googleAuthService);
								}
								public Dictionary<string, IOAuthService> Services { get; set; }
				}
}
