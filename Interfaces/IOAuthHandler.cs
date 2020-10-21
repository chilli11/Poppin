using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
				public interface IOAuthHandler
				{
								public Dictionary<string, IOAuthService> Services { get; set; }
				}
}
