

using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Data
{
    public class UserDbContext: IdentityDbContext<IdentityUser>
    {
        public UserDbContext() :
        base("ConnectionStrings")
        {
            this.Configuration.ProxyCreationEnabled = false;
        }
    }
}
