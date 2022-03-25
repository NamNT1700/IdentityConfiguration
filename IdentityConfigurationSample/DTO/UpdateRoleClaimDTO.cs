using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Data
{
    public class UpdateRoleClaimData
    {
        public string RoleName { get; set; }
        public UpdateRoleClaimDTO UpdateUserClaimDTO { get; set; }
    }
    public class UpdateRoleClaimDTO
    {
        public List<string> RoleClaims { get; set; }
    }
}
