using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.DTO
{
    public class UpdateRoleData
    {
        public string Name { get; set; }
        public UpdateRoleDTO UpdateRoleDTO { get; set; }
    }
    public class UpdateRoleDTO
    {
        public string newName { get; set; }
    }
}
