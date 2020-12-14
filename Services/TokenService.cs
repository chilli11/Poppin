using Microsoft.IdentityModel.Tokens;
using Poppin.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Services
{
    public class TokenService : ITokenService
    {
        public TokenValidationParameters TokenValidationParameters { get; set; }
    }
}
