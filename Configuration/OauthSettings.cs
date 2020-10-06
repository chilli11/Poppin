using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Configuration
{
				public class OauthSettings
				{
								public OauthKeySet Google { get; set; }
				}

				public class OauthKeySet
				{
								public string Secret { get; set; }
								public string ClientId { get; set; }
				}
}
