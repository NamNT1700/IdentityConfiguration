using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.DTO
{
    public class UpdateUserData
    {
        public string UseName { get; set; }
        public string PassWord { get; set;}
        public UpdateUsersDTO UpdateUsersDTO { get; set; }
      
    }
    public class UpdateUsersDTO
    {
        public  string PhoneNumber { get; set; }
        public  string Email { get; set; }
    }
}
