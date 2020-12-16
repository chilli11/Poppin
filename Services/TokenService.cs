using Microsoft.IdentityModel.Tokens;
using Poppin.Configuration;
using Poppin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Poppin.Services
{
    public class TokenService : ITokenService
    {
        public TokenService(IJwtSettings jwtSettings)
        {
            TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(jwtSettings.Secret)),
                ValidateIssuer = false,
                ValidateAudience = false,
                RequireExpirationTime = false, // needs change eventually
                ValidateLifetime = true
            };
        }
        public TokenValidationParameters TokenValidationParameters { get; set; }
    }
}
