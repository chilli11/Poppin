using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Contracts.Requests
{
				/// <summary>
				/// Revokes referenced <see cref="Models.Identity.RefreshToken" />
				/// </summary>
				public class RevokeTokenRequest
				{
								public string Token { get; set; }
				}
}
