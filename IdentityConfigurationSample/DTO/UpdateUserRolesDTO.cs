using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Data
{
    public class UpdateUserRole
    {
        public string id { get; set; }
        public UpdateUserRolesDTO UpdateUserRolesDTO { get; set; }
    }
    public class UpdateUserRolesDTO
    {
        public List<string> roles { get; set; }
    }
}
