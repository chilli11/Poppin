using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Poppin.Interfaces
{
    interface ITokenService
    {
        public TokenValidationParameters TokenValidationParameters { get; set; }
    }
}
