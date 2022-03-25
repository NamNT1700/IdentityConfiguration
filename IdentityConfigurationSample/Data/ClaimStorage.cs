using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Data
{
    public static class ClaimStorage
    {
        public static List<Claim> AllClaim = new List<Claim>()
        {
            new Claim("manageUser","manageUser"),               //admin
            new Claim("manageUser","manageRole"),               //admin
            //new Claim("admin","manageUser"),               
            //new Claim("admin","manageRole"),
            //new Claim("",""),
            //new Claim("",""),
        };
    }
}
