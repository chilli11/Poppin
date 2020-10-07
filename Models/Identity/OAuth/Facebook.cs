using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity.OAuth
{
				public class Facebook
				{
        public class AccessTokenResult : IAccessTokenResult
        {
            [JsonProperty("access_token")]
            public string AccessToken { get; set; }
            [JsonProperty("token_type")]
            public string TokenType { get; set; }
            [JsonProperty("expires_in")]
            public long Expires { get; set; }
        }
        public class TokenValidationResult
        {
            [JsonProperty("data")]
            public FacebookValidationData Data { get; set; }
        }

        public class FacebookValidationData : IValidationData
        {
            [JsonProperty("app_id")]
            public string AppId { get; set; }

            [JsonProperty("type")]
            public string AccessType { get; set; }

            [JsonProperty("application")]
            public string ApplicationName { get; set; }

            [JsonProperty("expires_at")]
            public long Expiration { get; set; }

            [JsonProperty("data_access_expires_at")]
            public long DataAccessExpiration { get; set; }

            [JsonProperty("is_valid")]
            public bool IsValid { get; set; }

            [JsonProperty("user_id")]
            public string UserId { get; set; }

            [JsonProperty("scopes")]
            public string[] Permissions { get; set; }

        }

        public class UserInfoResult : IUserInfoResult
								{
            public string Email { get; set; }
            [JsonProperty("first_name")]
            public string FirstName { get; set; }
            [JsonProperty("last_name")]
            public string LastName { get; set; }
								}
				}
}
