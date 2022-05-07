using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Data
{
    //[Table("Tokens")]
    public class Tokens
    {
        //[Key]
        //public Guid id { get; set; }
        public string accessToken { get; set; }
        //public string refreshToken { get; set; }
        public IList<string> Roles { get; set; }
        public string id { get; set; }

        //public string UserId { get; set; }
        //public IdentityUser identityUser { get; set; } 
    }
}
