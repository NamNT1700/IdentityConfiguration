using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Authentication
{
    public  interface ITokenManager
    {
        public Task<string> GenerateToken(IdentityUser user);
    }
}
