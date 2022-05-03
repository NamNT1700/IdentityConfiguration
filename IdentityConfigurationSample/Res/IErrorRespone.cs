using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace IdentityConfigurationSample.Res
{
    public interface IErrorResponse
    {
         string status { get; set; }
         List< string> Description { get; set; }
    }
}
