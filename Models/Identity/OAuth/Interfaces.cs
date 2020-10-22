using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Models.Identity.OAuth
{
    public interface IAccessTokenResult
				{
        public string AccessToken { get; set; }
        public string TokenType { get; set; }
        public long Expires { get; set; }
    }
    public interface ITokenValidationResult
    {
        public IValidationData Data { get; set; }
    }

    public interface IValidationData
    {
        public string AppId { get; set; }
        public string AccessType { get; set; }
        public long Expiration { get; set; }
        public bool IsValid { get; set; }
        public string UserId { get; set; }
        public string[] Permissions { get; set; }
    }

    public interface IUserInfoResult
				{
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PictureUrl { get; set; }
				}
}
