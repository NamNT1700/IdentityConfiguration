using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.DTO
{
    public class GetAllTotal
    {
        public int total { get; set; }
        public IEnumerable<UserDTO> users { get; set; }
    }
}
