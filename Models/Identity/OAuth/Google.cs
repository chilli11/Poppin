using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity.OAuth
{
				public class Google
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
            public GoogleValidationData Data { get; set; }
        }

        public class GoogleValidationData : IValidationData
        {
            [JsonProperty("app_id")]
            public string AppId { get; set; }

            [JsonProperty("type")]
            public string AccessType { get; set; }

            [JsonProperty("application")]
            public string ApplicationName { get; set; }

            [JsonProperty("data_access_expires_at")]
            public long Expiration { get; set; }

            [JsonProperty("is_valid")]
            public bool IsValid { get; set; }

            [JsonProperty("scopes")]
            public string[] Permissions { get; set; }

            [JsonProperty("user_id")]
            public string UserId { get; set; }
            [JsonProperty("metadata")]
            public Dictionary<string, string> Context { get; set; }
        }

        public class UserInfoResult : IUserInfoResult
        {
            public string Email { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
        }
    }
}
