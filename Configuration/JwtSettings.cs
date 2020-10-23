using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Configuration
{
				public class JwtSettings : IJwtSettings
				{
								public string Secret { get; set; }
				}

				public interface IJwtSettings
				{
								public string Secret { get; set; }
				}
}
