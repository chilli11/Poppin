using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Configuration
{
				public class OAuthSettings
				{
								public OAuthKeySet Google { get; set; }
								public OAuthKeySet Facebook { get; set; }
				}

				public class OAuthKeySet
				{
								public string Secret { get; set; }
								public string ClientId { get; set; }
				}
}
