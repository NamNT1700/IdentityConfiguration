using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Data
{
    public class UserData
    {
        public string UserName { get; set; }
        public UpdateUserRolesDTO UpdateUserRolesDTO { get; set; }
    }
    public class UpdateUserRolesDTO
    {
        public List<string> Roles { get; set; }
    }
}
