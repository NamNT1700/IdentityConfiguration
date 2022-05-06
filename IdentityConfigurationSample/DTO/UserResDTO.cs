using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.DTO
{
    public class UserResDTO
    {
        public string id { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string accsesstoken { get; set; }
        public IList<string> roles { get; set; }
    }
}
