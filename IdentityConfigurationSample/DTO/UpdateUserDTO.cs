using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.DTO
{
    public class UpdateUserData
    {
        public string UserName { get; set; }
        //public string PassWord { get; set;}
        public UpdateUserDTO UpdateUserDTO { get; set; }
      
    }
    public class UpdateUserDTO
    {
        public  string PhoneNumber { get; set; }
        public  string Email { get; set; }
    }
}
